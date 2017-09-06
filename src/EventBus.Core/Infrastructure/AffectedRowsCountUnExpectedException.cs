using System;

namespace EventBus.Core.Infrastructure
{
    public class AffectedRowsCountUnExpectedException : Exception
    {
        public AffectedRowsCountUnExpectedException(int expected, int actually) : base($"Sql execution should affect {expected} rows but affected {actually} rows actually.")
        {
            Expected = expected;
            Actually = actually;
        }

        public int Expected { get;  }
        public int Actually { get;  }
    }
}
