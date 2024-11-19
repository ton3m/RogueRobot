﻿namespace Assets.CourseGame.Develop.CommonServices.DataManagment
{
    public interface IDataRepository
    {
        string Read(string key);
        void Write(string key, string serializedData);
        void Remove(string key);
        bool Exists(string key);
    }
}
