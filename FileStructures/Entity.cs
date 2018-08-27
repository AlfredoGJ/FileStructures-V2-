using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class Entity
    {
        private DataFileManager dataManager;

        public string Name
        {
            get => default(int);
            set
            {
            }
        }

        public long Position
        {
            get => default(int);
            set
            {
            }
        }

        public long AttributesPtr
        {
            get => default(int);
            set
            {
            }
        }

        public long DataPtr
        {
            get => default(int);
            set
            {
            }
        }

        public long NextPtr
        {
            get => default(int);
            set
            {
            }
        }

        public List<Attribute> Attributes
        {
            get => default(int);
            set
            {
            }
        }

        public object PrimaryKeyAttribute
        {
            get => default(int);
            set
            {
            }
        }

        public List<DataRegister> Registers
        {
            get => default(int);
            set
            {
            }
        }

        public bool Edited
        {
            get => default(int);
            set
            {
            }
        }

        public void AddAttribute()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAttribute()
        {
            throw new System.NotImplementedException();
        }

        public void SetPrimaryKeyAttribute()
        {
            throw new System.NotImplementedException();
        }

        public void AddRegister()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteRegister()
        {
            throw new System.NotImplementedException();
        }
    }
}