using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    [Serializable]
    public class Project
    {
        public string Name { get;private  set; }
        public List<Entity> Entities { get; set; }

        
        public Project()
        { }

        public Project(string projectName)
        {
            Name = projectName;
            Entities = new List<Entity>();
        }

        internal void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        internal void DeleteEntity(Entity entity)
        {
            Entities.Remove(entity);
        }
    }
}
