﻿using System;
using System.Collections.Generic;
using System.Linq;

using Framework.Configuration.BLL.SubscriptionSystemService3.Lambdas;
using Framework.Configuration.Core;
using Framework.Configuration.Domain;
using Framework.Notification;
using Framework.Persistent;

using JetBrains.Annotations;

namespace Framework.Configuration.BLL.SubscriptionSystemService3.Recipients
{
    /// <summary>
    ///     Компонент для поиска получателей уведомлений, указанных в подписке ламбда-выражением типа "Generation".
    /// </summary>
    public class GenerationRecipientsResolver<TBLLContext>
        where TBLLContext : class
    {
        private readonly LambdaProcessorFactory<TBLLContext> lambdaProcessorFactory;

        /// <summary>Создаёт экземпляр класса <see cref="GenerationRecipientsResolver" />.</summary>
        /// <param name="lambdaProcessorFactory">Фабрика процессоров лямбда-выражений.</param>
        /// <exception cref="System.ArgumentNullException">Аргумент lambdaProcessorFactory равен null.</exception>
        public GenerationRecipientsResolver([NotNull] LambdaProcessorFactory<TBLLContext> lambdaProcessorFactory)
        {
            if (lambdaProcessorFactory == null)
            {
                throw new ArgumentNullException(nameof(lambdaProcessorFactory));
            }

            this.lambdaProcessorFactory = lambdaProcessorFactory;
        }

        /// <summary>Выполняет поиск получателей уведомлений по подписке.</summary>
        /// <typeparam name="T">Тип доменного объекта.</typeparam>
        /// <param name="subscription">Подписка.</param>
        /// <param name="versions">Версии доменного объекта.</param>
        /// <returns>Коллекцию экземпляров <see cref="RecipientsResolverResult" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Аргумент
        ///     subscription
        ///     или
        ///     versions равен null.
        /// </exception>
        public virtual IEnumerable<RecipientsResolverResult> Resolve<T>(
            [NotNull] Subscription subscription,
            [NotNull] DomainObjectVersions<T> versions)
            where T : class
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            if (versions == null)
            {
                throw new ArgumentNullException(nameof(versions));
            }

            var toProcessor = this.lambdaProcessorFactory.Create<GenerationLambdaProcessorTo<TBLLContext>>();
            var ccProcessor = this.lambdaProcessorFactory.Create<GenerationLambdaProcessorCc<TBLLContext>>();
            var replyToProcessor = this.lambdaProcessorFactory.Create<GenerationLambdaProcessorReplyTo<TBLLContext>>();

            var toInfos = toProcessor.Invoke(subscription, versions).ToList();
            var ccInfos = ccProcessor.Invoke(subscription, versions).ToList();
            var replyToInfos = replyToProcessor.Invoke(subscription, versions).ToList();

            var results = this.CreateResults(toInfos, ccInfos, replyToInfos);

            return results;
        }

        private static IEnumerable<RecipientsResolverResult> CollapseResults(
            IEnumerable<RecipientsResolverResult> rawResults)
        {
            var groups = rawResults.GroupBy(r => r.DomainObjectVersions);
            var results = groups.Select(CreateResultFromGroup).ToList();

            return results;
        }

        private static RecipientsResolverResult CreateResultFromGroup(
            IGrouping<DomainObjectVersions<object>, RecipientsResolverResult> group)
        {
            var to = new RecipientCollection(@group.SelectMany(r => r.RecipientsBag.To));
            var cc = new RecipientCollection(@group.SelectMany(r => r.RecipientsBag.Cc));
            var replyTo = new RecipientCollection(@group.SelectMany(r => r.RecipientsBag.ReplyTo));
            var bag = new RecipientsBag(to, cc, replyTo);
            var result = new RecipientsResolverResult(bag, @group.Key);

            return result;
        }

        private IEnumerable<RecipientsResolverResult> CreateResults(
            IEnumerable<NotificationMessageGenerationInfo> toInfos,
            IEnumerable<NotificationMessageGenerationInfo> ccInfos,
            IEnumerable<NotificationMessageGenerationInfo> replyTo)
        {
            var intermediateResults = toInfos
                .Select(this.CreateToResult)
                .Concat(ccInfos.Select(this.CreateCcResult))
                .Concat(replyTo.Select(this.CreateReplyToResult))
                .ToList();

            var results = CollapseResults(intermediateResults);
            return results;
        }

        private RecipientsResolverResult CreateCcResult(NotificationMessageGenerationInfo info)
        {
            var recipients = info.Recipients.Select(this.CreateRecipient);
            var objectVersions = new DomainObjectVersions<object>(info.PreviousRoot, info.CurrentRoot);
            var bag = new RecipientsBag(new RecipientCollection(), new RecipientCollection(recipients), new RecipientCollection());
            var result = new RecipientsResolverResult(bag, objectVersions);
            return result;
        }

        private RecipientsResolverResult CreateReplyToResult(NotificationMessageGenerationInfo info)
        {
            var recipients = info.Recipients.Select(this.CreateRecipient);
            var objectVersions = new DomainObjectVersions<object>(info.PreviousRoot, info.CurrentRoot);
            var bag = new RecipientsBag(new RecipientCollection(), new RecipientCollection(), new RecipientCollection(recipients));
            var result = new RecipientsResolverResult(bag, objectVersions);
            return result;
        }


        private RecipientsResolverResult CreateToResult(NotificationMessageGenerationInfo info)
        {
            var recipients = info.Recipients.Select(this.CreateRecipient);
            var objectVersions = new DomainObjectVersions<object>(info.PreviousRoot, info.CurrentRoot);
            var bag = new RecipientsBag(new RecipientCollection(recipients), new RecipientCollection(), new RecipientCollection());
            var result = new RecipientsResolverResult(bag, objectVersions);
            return result;
        }

        private Recipient CreateRecipient(IEmployee employee)
        {
            return new Recipient(employee.Login, employee.Email);
        }
    }
}
