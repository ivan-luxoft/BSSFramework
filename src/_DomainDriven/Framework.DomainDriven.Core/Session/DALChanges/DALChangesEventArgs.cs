﻿using System;

using JetBrains.Annotations;

namespace Framework.DomainDriven
{
    public class DALChangesEventArgs : EventArgs
    {
        public DALChangesEventArgs([NotNull] DALChanges changes)
        {
            this.Changes = changes ?? throw new ArgumentNullException(nameof(changes));
        }


        public DALChanges Changes { get; }
    }
}
