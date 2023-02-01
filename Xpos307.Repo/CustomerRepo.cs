using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xpos307.DataModels;
using Xpos307.ViewModels;
using System.ComponentModel;
using System.Data;

namespace Xpos307.Repo
{
    public class CustomerRepo
    {
        private readonly XPOS307Context db;
        private readonly VMResponse response=new VMResponse();
        private readonly int userId = 1;

        public CustomerRepo(XPOS307Context _db)
        {
            db = _db;
        }
        
        public VMResponse GetAll()
        {
            try
            {
                List<VMTblCustomer> DataView = (
                from c in db.TblCustomer
                where c.IsDelete == false
                select new VMTblCustomer
                {
                    Id = c.Id,
                    Name = c.Name,
                    Password = c.Password,
                    Address = c.Address,
                    Phone = c.Phone,

                    RoleId = c.RoleId,


                    IsDelete = c.IsDelete,
                    CreateBy = c.CreateBy,
                    CreateDate = c.CreateDate,
                    UpdateBy = c.UpdateBy,
                    UpdateDate = c.UpdateDate
                }).ToList();
                response.message = (DataView.Count > 0)
                     ? $"Customer data Successfully fetched!"
                     : $"Customer data is not available!";
                response.entity = DataView;
            }
            catch(Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;
        } 

        public VMResponse GetByID(int CustomerId)
        {
            try
            {
                VMTblCustomer DataView = (
                from c in db.TblCustomer
                where c.Id == CustomerId && c.IsDelete == false
                select new VMTblCustomer
                {
                    Id = c.Id,
                    Name = c.Name,
                    Password = c.Password,
                    Address = c.Address,
                    Phone = c.Phone,
                    RoleId = c.RoleId,


                    IsDelete = c.IsDelete,
                    CreateBy = c.CreateBy,
                    CreateDate = c.CreateDate,
                    UpdateBy = c.UpdateBy,
                    UpdateDate = c.UpdateDate
                }).SingleOrDefault();
                response.message = (DataView != null)
                     ? $"Customer data Successfully fetched!"
                     : $"Customer data is not available!";
                response.entity = DataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
                return response;
        }

        public VMResponse Edit(VMTblCustomer dataView)
        {
            try
            {
                TblCustomer DataModel = db.TblCustomer.Find(dataView.Id);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {dataView.Id} is not available!";
                }
                else
                {
                    DataModel.Id= dataView.Id;
                    DataModel.Name= dataView.Name;
                    //DataModel.Password= dataView.Password;
                    DataModel.Address= dataView.Address;
                    DataModel.Phone= dataView.Phone;
                    DataModel.RoleId= dataView.RoleId;
                    DataModel.UpdateBy= dataView.UpdateBy;
                    DataModel.UpdateDate= DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = "Data Succesfully Update";
                    response.entity = dataView;
                }
            }
            catch(Exception e)
            {
                response.Success = false;
                response.message = "Error Edit Data" + e.Message;
            }
            return response;
        }

        public VMResponse Add(VMTblCustomer dataView)
        {
            try
            {
                TblCustomer dataModel = new TblCustomer();
              
                dataModel.Name= dataView.Name;
                dataModel.Password= dataView.Password;
                dataModel.Address= dataView.Address;
                dataModel.Phone= dataView.Phone;
                dataModel.RoleId= dataView.RoleId;

                dataModel.CreateBy = dataView.CreateBy;
                dataModel.CreateDate= DateTime.Now;

                db.Add(dataModel);
                db.SaveChanges();

                response.message = "Data Berhasil Ditambahkan!";
                response.entity = dataView;
            }
            catch(Exception e)
            {
                response.Success = false;
                response.message= "Data ditambahkan Error"+e.Message;
                
            }
            return response;
        }

        public VMResponse Delete(int customerId, int updateBy)
        {
            try
            {
                TblCustomer DataModel = db.TblCustomer.Find(customerId);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {DataModel.Id} is not available!";
                }
                else
                {
                    DataModel.IsDelete = true;
                    DataModel.UpdateBy = updateBy;
                    DataModel.UpdateDate = DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = "Data Succesfully Delete!";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Delete Error!" + e.Message;
            }
            return response;
        }
    }
}
