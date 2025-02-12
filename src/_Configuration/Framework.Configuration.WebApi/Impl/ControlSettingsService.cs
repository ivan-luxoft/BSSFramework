﻿using System;
using System.Collections.Generic;
using System.Linq;

using Framework.Core;

using Framework.Configuration.BLL;
using Framework.Configuration.Domain;
using Framework.Configuration.Generated.DTO;
using Framework.DomainDriven;
using Framework.Persistent;

namespace Framework.Configuration.WebApi
{
    public partial class ConfigSLJsonController
    {
        [Microsoft.AspNetCore.Mvc.HttpPost(nameof(GetControlSettings))]
        public ControlSettingsRichDTO GetControlSettings(string name)
        {
            return this.Evaluate(DBSessionMode.Read, evaluateData =>
                                                         new ControlSettingsBLL(evaluateData.Context)
                                                             .GetRootControlSettingsForCurrentPrincipal(name)
                                                             .Maybe(controlSettings => controlSettings.ToRichDTO(evaluateData.MappingService)));
        }

        [Microsoft.AspNetCore.Mvc.HttpPost(nameof(SaveControlSettingsList))]
        public void SaveControlSettingsList(List<ControlSettingsStrictDTO> settings)
        {
            this.Evaluate(DBSessionMode.Write, (DomainDriven.ServiceModel.Service.EvaluatedData<IConfigurationBLLContext, IConfigurationDTOMappingService> evaluateData) =>
            {
                var customMappingService = new DontCheckIdConfigurationPrimitiveDTOMappingService(evaluateData.Context);

                var bll = new ControlSettingsBLL(evaluateData.Context);

                var ids = settings.Select(z => z.Id).ToList();

                var parentIds = settings.Where(z => !z.Parent.IsDefault()).Select(z => z.Parent.Id).ToList();

                var domainObjects = bll.GetListBy(z => ids.Contains(z.Id));
                var parentObjects = bll.GetListBy(z => parentIds.Contains(z.Id));
                var joinedList = settings.GroupJoin(domainObjects, z => z.Id, z => z.Id,
                                                    (z, w) =>
                                                    new { DTO = z, DomainObject = w.FirstOrDefault() })
                                         .ToArray();

                foreach (var pair in joinedList.Where(z => null != z.DomainObject))
                {
                    var dto = pair.DTO;
                    var domain = pair.DomainObject;

                    dto.MapToDomainObject(customMappingService, domain);

                    domain.ControlSettingsParams.Merge(dto.ControlSettingsParams, s => s.Id, t => t.Id,
                                                       t =>
                                                       new Framework.Configuration.Domain.
                                                           ControlSettingsParam(domain),
                                                       domain.RemoveDetails,
                                                       (s, t) => s.MapToDomainObject(customMappingService, t));
                    if (dto.Parent.Id != domain.Parent.TryGetId())
                    {
                        throw new NotSupportedException(
                            "Changing control setting parent object is not supported");
                    }
                    bll.Save(domain);
                }

                var createdObjects = joinedList.Where(z => null == z.DomainObject)
                    .Select
                    (
                        w =>
                        {
                            var dto = w.DTO;
                            var domain = new ControlSettings(null, evaluateData.Context.Authorization.RunAsManager.PrincipalName);
                            dto.MapToDomainObject(customMappingService, domain);
                            //if (string.IsNullOrWhiteSpace(domain.AccountName))
                            //{
                            //    domain.AccountName = Common.UserEnvironment.CurrentName;
                            //}
                            domain.ControlSettingsParams.Merge(dto.ControlSettingsParams,
                                                               s => s.Id, t => t.Id,
                                                               t =>
                                                               new Framework.Configuration.
                                                                   Domain.
                                                                   ControlSettingsParam(domain),
                                                               domain.RemoveDetails,
                                                               (s, t) => s.MapToDomainObject(customMappingService, t));
                            return new { DTO = dto, Domain = domain };
                        }
                    ).ToList();
                var parentsDictionary = parentObjects.ToDictionary(z => z.Id);
                foreach (var createdTuple in createdObjects)
                {
                    parentsDictionary.Add(createdTuple.DTO.Id, createdTuple.Domain);
                }
                parentsDictionary.Add(Guid.Empty, null);
                foreach (var createdTuple in createdObjects)
                {
                    var domain = createdTuple.Domain;
                    var parent = parentsDictionary[createdTuple.DTO.Parent.Id];
                    if (null != parent)
                    {
                        if (parent.Id == Guid.Empty)
                        {
                            throw new System.ArgumentException("Incorrect sequence of ControlSettings in the list. Parents should goo first");
                        }
                        bll.AddChild(parent, domain);
                    }

                    bll.Insert(domain, createdTuple.DTO.Id);
                }
            });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost(nameof(RemoveControlSettingsCollection))]
        public void RemoveControlSettingsCollection(List<ControlSettingsIdentityDTO> controlSettingsIdCollection)
        {
            this.Evaluate(DBSessionMode.Write, evaluateData =>
            {
                var bll = new ControlSettingsBLL(evaluateData.Context);
                var removedDomainObjects = controlSettingsIdCollection.Select(z => bll.GetById(z.Id, true)).ToList();
                foreach (var removedDomainObject in removedDomainObjects)
                {
                    bll.Remove(removedDomainObject);
                }
            });
        }
    }
}
