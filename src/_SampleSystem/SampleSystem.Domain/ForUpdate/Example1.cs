﻿using System;
using System.Collections.Generic;

using Framework.DomainDriven.BLL;
using Framework.Persistent;

namespace SampleSystem.Domain
{
    [BLLViewRole, BLLSaveRole(SaveType = BLLSaveType.Both)]
    [SampleSystemViewDomainObject(SampleSystemSecurityOperationCode.LocationView)]
    [SampleSystemEditDomainObject(SampleSystemSecurityOperationCode.LocationEdit)]
    public class Example1 : AuditPersistentDomainObjectBase, IMaster<Example2>
    {
        private readonly ICollection<Example2> items2 = new HashSet<Example2>();

        private Guid field1;

        private Guid field2;

        private Guid field3;

        public virtual IEnumerable<Example2> Items2 => this.items2;

        public virtual Guid Field1
        {
            get { return this.field1; }
            set { this.field1 = value; }
        }

        public virtual Guid Field2
        {
            get { return this.field2; }
            set { this.field2 = value; }
        }

        [SampleSystemViewDomainObject(SampleSystemSecurityOperationCode.LocationView)]
        [SampleSystemEditDomainObject(SampleSystemSecurityOperationCode.LocationEdit)]
        public virtual Guid Field3
        {
            get { return this.field3; }
            set { this.field3 = value; }
        }

        ICollection<Example2> IMaster<Example2>.Details => (ICollection<Example2>)this.Items2;
    }
}
