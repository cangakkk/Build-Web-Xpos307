using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xpos307.DataModels;
using Xpos307.ViewModels;
using System.Data;

namespace Xpos307.Repo
{
    public class OrderRepo
    {
        private readonly XPOS307Context db;
        private readonly VMResponse response = new VMResponse();
        private readonly int userId = 1;
        private readonly int customerId = 3;
        public OrderRepo(XPOS307Context _db)
        {
            db = _db;
        }

        public VMResponse GetAllTrxDetail()
        {
            try
            {
                List<VMTblOrderDetail> dataView = (
                     from oh in db.TblOrderHeader
                     join od in db.TblOrderDetail
                         on oh.Id equals od.OrderHeaderId
                     join p in db.TblProduct
                         on od.ProductId equals p.Id
                     join c in db.TblCustomer
                         on oh.CustomerId equals c.Id
                     where oh.IsDelete == false
                     select new VMTblOrderDetail
                     {
                         OrderHeaderId = oh.Id,
                         TrxCode = oh.TrxCode,
                         Amount = oh.Amount,
                         totalQty = oh.TotalQty,

                         Id = od.Id,
                         ProductId = od.ProductId,
                         ProductName = p.Name,
                         Price = od.Price,
                         Qty = od.Qty,

                         CustomerId = c.Id,
                         CustomerName= c.Name,

                         CreateBy = od.CreateBy,
                         CreateDate = od.CreateDate,
                         UpdateBy = od.UpdateBy,
                         UpdateDate = od.UpdateDate,
                     }
                 ).ToList();

                response.message = (dataView.Count > 0)
                    ? $"Order data Successfully fetched!"
                    : $"Order data is not available!";
                response.entity = dataView;

            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;

        }

        public VMResponse GetAllTrx()
        {
            try
            {
                List<VMTblOrderHeader> dataView = (
                        from oh in db.TblOrderHeader
                        where oh.IsDelete == false
                        select new VMTblOrderHeader
                        {
                            Id = oh.Id,
                            TrxCode = oh.TrxCode,
                            TotalQty = oh.TotalQty,
                            Amount = oh.Amount,
                            IsCheckout = oh.IsCheckout,

                            CustomerId = oh.CustomerId,

                            CreateBy = oh.CreateBy,
                            CreateDate = oh.CreateDate,
                            UpdateBy = oh.UpdateBy,
                            UpdateDate = oh.UpdateDate,
                        }
                    ).ToList();

                response.message = (dataView.Count > 0)
                   ? $"Order data Successfully fetched!"
                   : $"Order data is not available!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;
        }


        private List<VMTblOrderDetail> GetDataByID(int orderHeaderId)
        {

            return (
            from oh in db.TblOrderHeader
            join od in db.TblOrderDetail
                on oh.Id equals od.OrderHeaderId
            join p in db.TblProduct
                on od.ProductId equals p.Id
            join c in db.TblCustomer
                on oh.CustomerId equals c.Id
            where oh.IsDelete == false && oh.Id == orderHeaderId
            select new VMTblOrderDetail
            {
                OrderHeaderId = oh.Id,
                TrxCode = oh.TrxCode,
                Amount = oh.Amount,
                totalQty = oh.TotalQty,
                Qty=od.Qty,

                Id = od.Id,
                ProductId = od.ProductId,
                ProductName = p.Name,
                Price = od.Price,

                CustomerId = c.Id,
                CustomerName = c.Name,

                CreateBy = od.CreateBy,
                CreateDate = od.CreateDate,
                UpdateBy = od.UpdateBy,
                UpdateDate = od.UpdateDate,
            }).ToList();
        }

        private string GenerateCode()
        {
            string trxCode = $"XA-{DateTime.Now.ToString("ddMMyyyy")}-";
            string digit = "";

            TblOrderHeader dataModel = db.TblOrderHeader.OrderByDescending(o => o.Id).FirstOrDefault();
            if (dataModel != null)
            {
                digit = (dataModel.Id + 1).ToString().PadLeft(5, '0');
            }
            else
            {
                digit = "00001";
            }
            return trxCode + digit;
        }

        public VMResponse AddOrder(List<VMTblOrderDetail> orderChart, int totalItem, int totalAmount,bool isCheckOut)
        {
            try
            {
                //Prepare Empty Order Header Table
                TblOrderHeader dataHeader = new TblOrderHeader();

                dataHeader.TrxCode = GenerateCode();
                dataHeader.TotalQty = totalItem;
                dataHeader.CustomerId = customerId;
                dataHeader.Amount = totalAmount;
                dataHeader.IsCheckout = isCheckOut;

                dataHeader.CreateBy = userId;
                dataHeader.CreateDate = DateTime.Now;

                //save data order header
                db.Add(dataHeader);
                db.SaveChanges();

                response.message = "Order Berhasil ditambahkan!";
                response.entity = orderChart;

                foreach (VMTblOrderDetail item in orderChart)
                {
                    //Menyiapkan tampungan order detail
                    TblOrderDetail dataDetail = new TblOrderDetail();
                    dataDetail.OrderHeaderId = dataHeader.Id;
                    dataDetail.ProductId = item.ProductId;
                    dataDetail.Qty = item.Qty;
                    dataDetail.Price = item.Price;

                    dataDetail.UpdateBy = userId;
                    dataDetail.UpdateDate = DateTime.Now;

                    try
                    {
                        //save data order detail
                        db.Add(dataDetail);
                        db.SaveChanges();

                        //Mengupdate Product Stock
                        TblProduct dataProduct = db.TblProduct.Find(item.ProductId);
                        if (dataProduct != null)
                        {
                            dataProduct.Stock -= item.Qty;
                            try
                            {
                                //save data product
                                db.Update(dataProduct);
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                response.Success = false;
                                response.message = "Failed to Update Product Stock!" + e.Message;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Success = false;
                        response.message = "Failed to Add Product Detail!" + e.Message;

                    }
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Failed to Add New Order!" + e.Message;
            }

            return response;
        }

        public VMResponse GetTrxDetailById(int orderHeaderId)
        {
            try
            {
                List<VMTblOrderDetail> orderView = GetDataByID(orderHeaderId);
                response.message = (orderView.Count > 0 )
                    ? $"Data with Id={orderHeaderId} Available" 
                    : $"Data with Id={orderHeaderId} No Available";
                response.entity = orderView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;
        }

        public VMResponse GetTrxById(int orderID)
        {
            try
            {
                VMTblOrderHeader dataView = (
                         from oh in db.TblOrderHeader
                         where oh.IsDelete == false && oh.Id == orderID
                         select new VMTblOrderHeader
                         {
                             Id = oh.Id,
                             TrxCode = oh.TrxCode,
                             TotalQty = oh.TotalQty,
                             Amount = oh.Amount,
                             IsCheckout = oh.IsCheckout,

                             CustomerId = oh.CustomerId,

                             CreateBy = oh.CreateBy,
                             CreateDate = oh.CreateDate,
                             UpdateBy = oh.UpdateBy,
                             UpdateDate = oh.UpdateDate,
                         }
                     ).FirstOrDefault();

                response.message = (dataView != null)
                   ? $"Order data Successfully fetched!"
                   : $"Order data is not available!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Update!" + e.Message;
            }
            return response;
        }

        public VMResponse Edit(VMTblOrderDetail dataView)
        {
            try
            {
                TblOrderDetail DataModel = db.TblOrderDetail.Find(dataView.Id);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {dataView.Id} is not available!";
                }
                else
                {
                    DataModel.Id = dataView.Id;
                    DataModel.OrderHeaderId = dataView.OrderHeaderId;
                    DataModel.ProductId = dataView.ProductId;
                    DataModel.Qty = dataView.Qty;

                    DataModel.Price = dataView.Price;
                    DataModel.UpdateBy = dataView.UpdateBy;
                    DataModel.UpdateDate = DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = "Data Succesfully Update";
                    response.entity = dataView;
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Update!" + e.Message;
            }
            return response;
        }

        

        public VMResponse Add(VMTblOrderDetail dataView)
        {
            try
            {
                TblOrderDetail dataModel = new TblOrderDetail();

                dataModel.Id = dataView.Id;
                dataModel.OrderHeaderId = dataView.OrderHeaderId;
                dataModel.ProductId = dataView.ProductId;
                dataModel.Qty = dataView.Qty;

                dataModel.Price = dataView.Price;
                dataModel.CreateBy = dataView.CreateBy;
                dataModel.CreateDate = DateTime.Now;

                db.Add(dataModel);
                db.SaveChanges();

                response.message = "Data baru berhasil ditambahkan!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Data ditambahkan Error" + e.Message;
            }
            return response;
        }

        //public VMResponse SaveOrder(List<VMTblOrderDetail> orderChart, int totalItems, int totalAmount)
        //{
        //    try
        //    {
        //        //Prepare Empty Order Header Table
        //        TblOrderHeader dataHeader = new TblOrderHeader();

        //        dataHeader.TrxCode = GenerateCode();
        //        dataHeader.TotalQty = totalItems;
        //        dataHeader.CustomerId = customerId;
        //        dataHeader.Amount = totalAmount;
        //        dataHeader.IsCheckout = false;

        //        dataHeader.CreateBy = userId;
        //        dataHeader.CreateDate = DateTime.Now;

        //        //save data order header
        //        db.Add(dataHeader);
        //        db.SaveChanges();

        //        response.message = "Order Berhasil ditambahkan!";
        //        response.entity = orderChart;

        //        foreach (VMTblOrderDetail item in orderChart)
        //        {
        //            //Menyiapkan tampungan order detail
        //            TblOrderDetail dataDetail = new TblOrderDetail();
        //            dataDetail.OrderHeaderId = dataHeader.Id;
        //            dataDetail.ProductId = item.ProductId;
        //            dataDetail.Qty = item.Qty;
        //            dataDetail.Price = item.Price;

        //            dataDetail.UpdateBy = userId;
        //            dataDetail.UpdateDate = DateTime.Now;

        //            try
        //            {
        //                //save data order detail
        //                db.Add(dataDetail);
        //                db.SaveChanges();

        //                //Mengupdate Product Stock
        //                TblProduct dataProduct = db.TblProduct.Find(item.ProductId);
        //                if (dataProduct != null)
        //                {
        //                    dataProduct.Stock -= item.Qty;
        //                    try
        //                    {
        //                        //save data product
        //                        db.Update(dataProduct);
        //                        db.SaveChanges();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        response.Success = false;
        //                        response.message = "Failed to Update Product Stock!" + e.Message;
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                response.Success = false;
        //                response.message = "Failed to Add Product Detail!" + e.Message;

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        response.Success = false;
        //        response.message = "Failed to Add New Order!" + e.Message;
        //    }

        //    return response;
        //}


        public VMResponse Delete(int orderHeaderId, int updateBy)
        {
            try
            {
                TblOrderHeader DataModel = db.TblOrderHeader.Find(orderHeaderId);
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

        public VMResponse UptadeOrderHead(int orderHeaderId, int totalQty, decimal Amount, bool isCheckOut, int userId)
        {
            try
            {
                //Prepare Empty Order Header Table
                TblOrderHeader dataHeader = db.TblOrderHeader.Find(orderHeaderId);
                //dataHeader.TrxCode = GenerateCode();
                dataHeader.TotalQty = totalQty;
                dataHeader.Amount = Amount;
                dataHeader.IsCheckout = isCheckOut;

                dataHeader.UpdateBy = userId;
                dataHeader.UpdateDate = DateTime.Now;

                //save data order header
                db.Update(dataHeader);
                db.SaveChanges();

                response.message = "Data Succesfully Update!";
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Failed to Add New Order!" + e.Message;
            }

            return response;
        }
    }
}
