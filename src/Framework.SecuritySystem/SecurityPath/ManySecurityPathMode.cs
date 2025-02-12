﻿namespace Framework.SecuritySystem
{
    public enum ManySecurityPathMode
    {
        /// <summary>
        /// Константа, определяющая фильтрация по объектам (в результирующую выборку попадают объекты с отсутсвующими коллекциями)
        /// </summary>
        Any,

        All,

        /// <summary>
        /// Константа, определяющая фильтрация только по наличию объектов (в результирующую выборку не попадают объекты с отсутсвующими коллекциями)
        /// </summary>
        AnyStrictly
    }
}
