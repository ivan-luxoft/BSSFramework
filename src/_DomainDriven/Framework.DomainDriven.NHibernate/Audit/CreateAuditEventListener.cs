﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Framework.DomainDriven.Audit;

using NHibernate.Event;

namespace Framework.DomainDriven.NHibernate.Audit
{
    [System.Obsolete("Use AuditInterceptor instead - #IADFRAME-693")]
    public class CreateAuditEventListener : AuditEventListenerBase, IPreInsertEventListener
    {
        public CreateAuditEventListener(IEnumerable<IAuditProperty> auditProperties)
            : base(auditProperties)
        {
        }

        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            this.SetAuditFields(@event, @event.State);

            return Task.FromResult(true);
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            return this.SetAuditFields(@event, @event.State);
        }
    }
}
