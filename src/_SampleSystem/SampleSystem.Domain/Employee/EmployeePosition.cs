﻿using Framework.Core;
using Framework.DomainDriven.BLL;
using Framework.Restriction;

namespace SampleSystem.Domain
{
    [BLLViewRole]
    [SampleSystemViewDomainObject(SampleSystemSecurityOperationCode.EmployeePositionView)]
    [SampleSystemEditDomainObject(SampleSystemSecurityOperationCode.EmployeePositionEdit)]
    [UniqueGroup]
    public class EmployeePosition : BaseDirectory, IExternalSynchronizable
    {
        private long externalId;
        private string englishName;
        private Location location;

        public virtual long ExternalId
        {
            get { return this.externalId; }
            set { this.externalId = value; }
        }

        [Required]
        [UniqueElement]
        public virtual Location Location
        {
            get { return this.location; }
            set { this.location = value; }
        }

        [Required]
        [UniqueElement]
        public virtual string EnglishName
        {
            get { return this.englishName.TrimNull(); }
            set { this.englishName = value.TrimNull(); }
        }
    }
}
