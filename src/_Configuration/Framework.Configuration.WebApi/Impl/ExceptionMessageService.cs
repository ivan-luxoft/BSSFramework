﻿using System;

using Framework.Configuration.Domain;
using Framework.Configuration.Generated.DTO;
using Framework.DomainDriven;
using Framework.DomainDriven.BLL;

namespace Framework.Configuration.WebApi
{
    public partial class ConfigSLJsonController
    {
        [Microsoft.AspNetCore.Mvc.HttpPost(nameof(SaveExceptionMessage))]
        public virtual ExceptionMessageIdentityDTO SaveExceptionMessage(ExceptionMessageStrictDTO exceptionMessageStrict)
        {
            if (exceptionMessageStrict == null)
            {
                throw new ArgumentNullException(nameof(exceptionMessageStrict));
            }

            return this.Evaluate(
                DBSessionMode.Write,
                evaluateData =>
                {
                    var emptyMessage = new ExceptionMessage { IsRoot = true, IsClient = true };

                    var mappedMessage = emptyMessage.WithMap(exceptionMessageStrict, evaluateData.MappingService);

                    evaluateData.Context.Logics.ExceptionMessage.Save(mappedMessage);

                    return mappedMessage.ToIdentityDTO();
                });
        }
    }
}
