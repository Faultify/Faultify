using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class EqualityTarget
    {
        public bool NotEqualOrdered(int lhs, int rhs)
        {
            if (lhs != rhs)
            {
                return true;
            }
            return false;
        }

        public bool EqualOrdered(int lhs, int rhs)
        {
            if (lhs == rhs)
            {
                return true;
            }
            return false;
        }

        public bool LessThanEqualOrdered(int lhs, int rhs)
        {
            if (lhs <= rhs)
            {
                return true;
            }
            return false;
        }

        public bool LessThanEqualUnOrdered(uint lhs, uint rhs)
        {
            if (lhs <= rhs)
            {
                return true;
            }
            return false;
        }

        public bool MoreThanEqualOrdered(int lhs, int rhs)
        {
            if (lhs >= rhs)
            {
                return true;
            }
            return false;
        }

        public bool MoreThanEqualUnOrdered(uint lhs, uint rhs)
        {
            if (lhs >= rhs)
            {
                return true;
            }
            return false;
        }
        
        public bool LessThanOrdered(int lhs, int rhs)
        {
            if (lhs < rhs)
            {
                return true;
            }
            return false;
        }

        public bool LessThanUnOrdered(uint lhs, uint rhs)
        {
            if (lhs < rhs)
            {
                return true;
            }
            return false;
        }

        public bool MoreThanOrdered(int lhs, int rhs)
        {
            if (lhs > rhs)
            {
                return true;
            }
            return false;
        }

        public bool MoreThanUnOrdered(uint lhs, uint rhs)
        {
            if (lhs > rhs)
            {
                return true;
            }
            return false;
        }
    }
}
