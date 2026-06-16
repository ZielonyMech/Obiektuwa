using System;
using System.Collections.Generic;
using System.Text;

namespace Obiektuwa.Classes {
    public class Repository<T> {

        protected readonly FileHandler<T> reader;
        protected List<T> ObjectList = new();

        public Repository(string? filepath = null) {
            string repositoryFilepath = filepath ?? Helpers.GetClassCacheDirectory<T>();
            reader = new FileHandler<T>(repositoryFilepath);

            ObjectList = reader.ReadFile() ?? new List<T>();
        }

        public Repository(List<T> objectList, string? filepath = null) {
            ArgumentNullException.ThrowIfNull(objectList);
            ObjectList = objectList;

            string repositoryFilepath = filepath ?? Helpers.GetClassCacheDirectory<T>();
            reader = new FileHandler<T>(repositoryFilepath);
        }

        public T? FindOne(Func<T, bool> conditions) {
            ArgumentNullException.ThrowIfNull(conditions);
            return ObjectList.FirstOrDefault(conditions);
        }

        public List<T> FindAll(Predicate<T> conditions) {
            ArgumentNullException.ThrowIfNull(conditions);
            return ObjectList.FindAll(conditions);
        }

        public List<T> GetAll() {
            return ObjectList;
        }

        public bool Remove(T obj) {
            ArgumentNullException.ThrowIfNull(obj);
            return ObjectList.Remove(obj);
        }

        public void Add(T obj) {
            ArgumentNullException.ThrowIfNull(obj);
            ObjectList.Add(obj);
        }

        public void BulkAdd(List<T> obj) {
            ArgumentNullException.ThrowIfNull(obj);
            ObjectList.AddRange(obj);
        }

        public void Save() {
            reader.WriteFile(ObjectList);
        }
    }
}
