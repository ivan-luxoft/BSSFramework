﻿using Framework.Validation;

namespace Framework.Authorization.Domain
{
    /// <summary>
    /// Базовый доменный объект
    /// </summary>
    /// <remarks>
    /// Для базового доменного объекта генерятся ДТО
    /// </remarks>
    [AvailableDecimalValidator]
    [AvailablePeriodValidator]
    [AvailableDateTimeValidator]
    [DefaultStringMaxLengthValidator]
    public abstract class DomainObjectBase
    {
    }
}