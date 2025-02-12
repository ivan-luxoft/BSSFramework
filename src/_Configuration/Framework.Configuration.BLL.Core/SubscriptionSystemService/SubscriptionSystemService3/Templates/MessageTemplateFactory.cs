﻿using System;
using System.Collections.Generic;
using System.Linq;

using Framework.Configuration.BLL.SubscriptionSystemService3.Recipients;
using Framework.Configuration.BLL.SubscriptionSystemService3.Services;
using Framework.Configuration.Core;
using Framework.Configuration.Domain;
using Framework.Core;
using Framework.Notification;

using JetBrains.Annotations;

using Serilog;

namespace Framework.Configuration.BLL.SubscriptionSystemService3.Templates
{
    /// <summary>
    ///     Фабрика, предназначенная для создания шаблонов уведомлений.
    ///     Реализует логику создания шаблонов в зависимости о списка получателей To и получателей Cc подписки.
    /// </summary>
    public class MessageTemplateFactory<TBLLContext>
        where TBLLContext : class
    {
        private readonly RecipientsResolver<TBLLContext> recipientsResolver;

        private readonly AttachmentsResolver<TBLLContext> attachmentResolver;

        private readonly ExcessTemplatesFilter templatesFilter;
        private readonly ILogger logger;

        /// <summary>Создаёт экземпляр класса <see cref="MessageTemplateFactory" />.</summary>
        /// <param name="recipientsResolver">
        ///     Экземпляр <see cref="RecipientsResolver" />
        ///     с помощью которого будет производиться поиск получателей уведомлений.
        /// </param>
        /// <param name="templatesFilter">
        ///     Экземпляр <see cref="ExcessTemplatesFilter" />
        ///     с помощью которого будет удаление дублирующихся шаблонов уведомлений.
        /// </param>
        /// <param name="configurationContextFacade">Фасад контекста конфигурации</param>
        /// <exception cref="ArgumentNullException">
        ///     Аргумент recipientsResolver или templatesFilter или configurationContextFacade равен null.
        /// </exception>
        public MessageTemplateFactory(
            [NotNull] RecipientsResolver<TBLLContext> recipientsResolver,
            [NotNull] AttachmentsResolver<TBLLContext> attachmentResolver,
            [NotNull] ExcessTemplatesFilter templatesFilter,
            [NotNull] ConfigurationContextFacade configurationContextFacade)
        {
            if (configurationContextFacade == null)
            {
                throw new ArgumentNullException(nameof(configurationContextFacade));
            }

            this.recipientsResolver = recipientsResolver ?? throw new ArgumentNullException(nameof(recipientsResolver));
            this.attachmentResolver = attachmentResolver ?? throw new ArgumentNullException(nameof(attachmentResolver));
            this.templatesFilter = templatesFilter ?? throw new ArgumentNullException(nameof(templatesFilter));

            this.logger = Log.Logger.ForContext(this.GetType());
        }

        /// <summary>Создаёт список шаблонов уведомлений.</summary>
        /// <typeparam name="T">Тип доменного объекта к которому привязана подписка.</typeparam>
        /// <param name="subscriptions">Подписки для которых нужно создать шаблоны уведомлений.</param>
        /// <param name="versions">Версии доменного объекта.</param>
        /// <returns>Список созданных шаблонов уведомлений.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Аргумент
        ///     subscription
        ///     или
        ///     versions равен null.
        /// </exception>
        /// <exception cref="AggregateException">
        ///     Список исключений, которые возникли при получении шаблонов уведомлений по каждой подписке.
        /// </exception>
        public virtual IEnumerable<MessageTemplateNotification> Create<T>(
            [NotNull] IEnumerable<Subscription> subscriptions,
            [NotNull] DomainObjectVersions<T> versions)
            where T : class
        {
            if (subscriptions == null)
            {
                throw new ArgumentNullException(nameof(subscriptions));
            }

            if (versions == null)
            {
                throw new ArgumentNullException(nameof(versions));
            }

            var exceptions = new List<Exception>();
            var templates = new List<MessageTemplateNotification>();

            foreach (var subscription in subscriptions)
            {
                try
                {
                    this.logger.Information("Create templates for subscription '{subscription}'.", subscription);

                    var createResult = this.Create(subscription, versions).ToList();
                    templates.AddRange(createResult);

                    this.logger.Information("For subscription '{subscription}' '{createResultCount}' templates has been created. Templates: '{createResult}'.",
                                            subscription,
                                            createResult.Count,
                                            createResult.Select(notification => notification).Join(", "));
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "Can't create template");
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            var result = this.FilterTemplates(templates);

            return result;
        }

        private IEnumerable<MessageTemplateNotification> FilterTemplates(
            IList<MessageTemplateNotification> templates)
        {
            this.logger.Information("Remove excess message templates. Incoming templates: {templates}", templates.Select(t => t.MessageTemplateCode).Join(", "));

            var result = this.templatesFilter.FilterTemplates(templates).ToList();

            this.logger.Information("Excess message templates has been removed. Outgoing templates: {result}", result.Select(t => t.MessageTemplateCode).Join(", "));

            return result;
        }

        private IEnumerable<MessageTemplateNotification> Create<T>(
            Subscription subscription,
            DomainObjectVersions<T> versions)
            where T : class
        {
            var resolverResults = this.recipientsResolver.Resolve(subscription, versions);
            var result = resolverResults.SelectMany(r => this.Create<T>(r, subscription)).ToList();

            return result;
        }

        private IEnumerable<MessageTemplateNotification> Create<TSourceDomainObjectType>(
            RecipientsResolverResult recipientsResolverResult,
            Subscription subscription)
        {
            var recipients = recipientsResolverResult.RecipientsBag;

            var versions = recipientsResolverResult.DomainObjectVersions;

            var sourceDomainObjectType = typeof(TSourceDomainObjectType);
            var newDomainObjectType = versions.DomainObjectType;
            var newDomainObjectVersions = this.CreateNewDomainObjectVersions(versions);

            var method = new Func<Subscription, DomainObjectVersions<object>, RecipientsBag, IEnumerable<MessageTemplateNotification>>(this.CreateImplicit<object, object>)
                .CreateGenericMethod(sourceDomainObjectType, newDomainObjectType);

            var result = method.Invoke(this, new[] { subscription, newDomainObjectVersions, recipients });

            return (IEnumerable<MessageTemplateNotification>)result;
        }

        private object CreateNewDomainObjectVersions<T>(DomainObjectVersions<T> versions)
            where T : class
        {
            var genericType = typeof(DomainObjectVersions<>).MakeGenericType(versions.DomainObjectType);
            var ctor = genericType.GetConstructors().Single();
            var result = ctor.Invoke(new object[] { versions.Previous, versions.Current });

            return result;
        }

        [UsedImplicitly]
        private IEnumerable<MessageTemplateNotification> CreateImplicit<TSourceDomainObjectType, TModelObjectType>(
            Subscription subscription,
            DomainObjectVersions<TModelObjectType> versions,
            RecipientsBag recipients)
            where TModelObjectType : class
            where TSourceDomainObjectType : class
        {
            var factory = this.CreateFactory(recipients);
            var attachments = this.attachmentResolver.Resolve(subscription, versions);
            var result = factory.Create<TSourceDomainObjectType, TModelObjectType>(versions, subscription, recipients, attachments);
            return result;
        }

        private MessageTemplateFactoryBase CreateFactory(RecipientsBag recipientsBag)
        {
            return recipientsBag.Cc.Any()
                ? (MessageTemplateFactoryBase)new MessageTemplateFactoryCc()
                : new MessageTemplateFactoryTo();
        }
    }
}
