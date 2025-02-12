﻿using System.IO;
using System.Text;

using Framework.Configuration.Core;
using Framework.Configuration.Domain;

using SampleSystem.BLL;

namespace SampleSystem.Subscriptions.Metadata.Examples.AttachmentTemplateEvaluator
{
    public sealed class AttachmentLambdaTemplateEvaluator : AttachmentLambdaBase<Domain.Employee>
    {
        public const string AttachmentName = "report.html";

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentLambdaTemplateEvaluator"/> class.
        /// </summary>
        public AttachmentLambdaTemplateEvaluator()
        {
            this.DomainObjectChangeType = Framework.Configuration.SubscriptionModeling.DomainObjectChangeType.Update;
            this.Lambda = GetAttachments;
        }

        private static System.Net.Mail.Attachment[] GetAttachments(
            ISampleSystemBLLContext context,
            DomainObjectVersions<Domain.Employee> versions)
        {
            var template = Encoding.UTF8.GetBytes($"Hello world! {versions.Current.NameNative}");
            
            return new[] { new System.Net.Mail.Attachment(new MemoryStream(template), AttachmentLambdaTemplateEvaluator.AttachmentName) };
        }
    }
}
