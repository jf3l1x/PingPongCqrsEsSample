﻿using System.Collections.Generic;
using CommonDomain;

namespace Pong.Services.Default
{
    public class NullConflictDetection : IDetectConflicts
    {
        public void Register<TUncommitted, TCommitted>(ConflictDelegate<TUncommitted, TCommitted> handler)
            where TUncommitted : class
            where TCommitted : class
        {
        }

        public bool ConflictsWith(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents)
        {
            return false;
        }
    }
}
