﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public interface IStudentRepository
    {
        void Add(Student student);
        Student FindByEmail(string email);
        Student FindById(int id);
        IQueryable<Student> FindAll();
        void SaveChanges();
        void Update(Student student, Student newStudent);

    }
}