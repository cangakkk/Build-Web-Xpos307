using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xpos307.DataModels;
using Xpos307.ViewModels;
using System.Data;


namespace Xpos307.Repo
{
    public class CategoryRepo
    {
        private readonly XPOS307Context db;
        private readonly VMResponse response=new VMResponse();
        private readonly int userId = 1;

        public CategoryRepo(XPOS307Context _db)
        {
            db = _db;
        }
        
        public VMResponse GetAll()
        {
            try
            {
                List<VMTblCategory> DataView = (
                from a in db.TblCategory
                where a.IsDelete == false
                select new VMTblCategory
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    IsDelete = a.IsDelete,
                    CreateBy = a.CreateBy,
                    CreateDate = a.CreateDate,
                    UpdateBy = a.UpdateBy,
                    UpdateDate = a.UpdateDate
                }).ToList();
                response.message = (DataView.Count > 0)
                    ? $"Category data Successfully fetched!"
                    : $"Category data is not available!";
                response.entity = DataView;
            }
            catch(Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;
        } 

        public VMResponse GetByName(string Name)
        {
            try
            {
                List<VMTblCategory> DataView = (
                from a in db.TblCategory
                where a.IsDelete == false && (a.Name+a.Description).Contains(Name)
                select new VMTblCategory
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    IsDelete = a.IsDelete,
                    CreateBy = a.CreateBy,
                    CreateDate = a.CreateDate,
                    UpdateBy = a.UpdateBy,
                    UpdateDate = a.UpdateDate

                }).ToList();
                response.message = (DataView.Count > 0)
                    ? $"Category data Successfully fetched!"
                    : $"Category data is not available!";
                response.entity = DataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }

            return response;
        }

        public VMResponse GetByID(int CantegoryID)
        {
            try
            {
                VMTblCategory DataView = (
                from a in db.TblCategory
                where a.IsDelete == false && a.Id == CantegoryID
                select new VMTblCategory
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    IsDelete = a.IsDelete,
                    CreateBy = a.CreateBy,
                    CreateDate = a.CreateDate,
                    UpdateBy = a.UpdateBy,
                    UpdateDate = a.UpdateDate
                }).SingleOrDefault();
                response.message = (DataView != null)
                    ? $"Category data Successfully fetched!"
                    : $"Category data is not available!";
                response.entity =DataView;
            }
            catch(Exception e ) 
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;
        }

        public VMResponse Edit(VMTblCategory dataView)
        {
            try
            {
                TblCategory DataModel = db.TblCategory.Find(dataView.Id);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {dataView.Id} is not available!";
                }
                else
                {
                    DataModel.Id = dataView.Id;
                    DataModel.Name = dataView.Name;
                    DataModel.Description= dataView.Description;

                    DataModel.UpdateBy = userId;
                    DataModel.UpdateDate = DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = "Data Succesfully Update!";
                    response.entity = dataView;
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Save Failed!" + e.Message;
            }
            return response;
        }

        public VMResponse Add(VMTblCategory dataView)
        {
            try
            {
                //prepare new table
                TblCategory dataModel = new TblCategory();

                //menambahakan data ke table
                dataModel.Name = dataView.Name;
                dataModel.Description = dataView.Description;

                dataModel.CreateBy=userId;
                dataModel.CreateDate= DateTime.Now;
                
                //save table
                db.Add(dataModel);
                db.SaveChanges();

                response.message = "Data Baru Berhasil Ditambahkan!";
                response.entity = dataView;

            }
            catch(Exception e)
            {
                response.Success = false;
                response.message= "Data Ditambahkan Error" + e.Message;
            }
            return response;
        }

        public VMResponse Delete(int categoryId, int updateBy)
        {
            try
            {
                TblCategory DataModel = db.TblCategory.Find(categoryId);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {DataModel.Id} is not available!";
                }
                else
                {
                    DataModel.IsDelete = true;
                    DataModel.UpdateBy= updateBy;
                    DataModel.UpdateDate= DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = "Data Succesfully Delete!";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Delete" + e.Message;
            }
            return response;
        }
    }
}
