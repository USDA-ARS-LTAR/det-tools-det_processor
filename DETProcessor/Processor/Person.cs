using System;
using System.Collections.Generic;
using System.Text;

namespace DETProcessor.Processor
{
    public class Person : IComparable<Person>
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool IsPI { get; set; }

        override public string ToString()
        {
            return LastName + ", " + FirstName;
        }

        public int CompareTo(Person other)
        {
            if ((IsPI && !other.IsPI) || (!IsPI && other.IsPI))
            {
                if (IsPI) return -1;
                else return 1;
            }
            else
            {
                int ret = LastName.CompareTo(other.LastName);
                if (ret == 0)
                {
                    return FirstName.CompareTo(other.FirstName);
                }
                else return ret;
            }
        }
    }

    public class PersonComparator : IEqualityComparer<Person>
    {
        public bool Equals(Person x, Person y)
        {
            return x.FirstName.Equals(y.FirstName) && x.LastName.Equals(y.LastName);
        }

        public int GetHashCode(Person obj)
        {
            return (obj.LastName + obj.FirstName).GetHashCode();
        }
    }
}
