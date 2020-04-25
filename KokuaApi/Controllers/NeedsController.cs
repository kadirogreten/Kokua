using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokuaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using MongoDB.Bson;
using ServiceLayer;
using GeoCoordinatePortable;

namespace KokuaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeedsController : ControllerBase
    {


        private readonly IUnitOfWork _uow;
        private readonly UserManager<KokuaUser> _userManager;
        private readonly SignInManager<KokuaUser> _signInManager;
        private readonly ILogger _logger;

        public NeedsController(UserManager<KokuaUser> userManager, SignInManager<KokuaUser> signInManager, ILogger<NeedsController> logger, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
        }



        #region helper

        private double GetDistance(LocationViewModel userLocation, LocationViewModel beneficiaryLocation)
        {

           
            GeoCoordinate coordinateUser = new GeoCoordinate();
            GeoCoordinate coordinateBeneficary = new GeoCoordinate();
            try
            {
                coordinateUser = new GeoCoordinate(Convert.ToDouble(userLocation.Latitude.Replace(",", ".")), Convert.ToDouble(userLocation.Longitude.Replace(",", ".")));
                coordinateBeneficary = new GeoCoordinate(Convert.ToDouble(beneficiaryLocation.Latitude.Replace(",", ".")), Convert.ToDouble(beneficiaryLocation.Longitude.Replace(",", ".")));
            }
            catch (Exception ex)
            {
                coordinateUser = new GeoCoordinate(double.Parse(userLocation.Latitude.Replace(".", ",")), double.Parse(userLocation.Longitude.Replace(".", ",")));
                coordinateBeneficary = new GeoCoordinate(double.Parse(beneficiaryLocation.Latitude.Replace(".", ",")), double.Parse(beneficiaryLocation.Longitude.Replace(".", ",")));
            }

            var distance = coordinateUser.GetDistanceTo(coordinateBeneficary);

            return distance;

        }

        #endregion



        [Route("Create-Needs")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> InsertNeed(AddNeedResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;

            if (model.Title != null)
            {

                var needsList = await _uow.Needs.WhereAsync(a => a.Title == model.Title && a.Username == username);

                if (needsList.ToList().Count > 0)
                {
                    return Ok(new { IsError = false, Result = "", Message = "This title already taken!" });
                }

                var need = new Needs
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    OrderStatus = OrderStatus.Waiting,
                    CreatedAt = DateTime.Now,
                    Title = model.Title,
                    Username = username,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                };





                if (need == null)
                {
                    return Ok(new { IsSuccess = false, Result = "", Message = "Need list return null value!" });
                }

                if (need.NeedProducts == null)
                {
                    need.NeedProducts = new List<NeedProducts>();
                }

                NeedProducts product;
                List<NeedProducts> liste = new List<NeedProducts>();
                foreach (var item in model.ProductDescription)
                {
                    product = new NeedProducts
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        NeedId = need.Id,
                        ProductDescription = item
                    };

                    liste.Add(product);

                    _uow.NeedProducts.Insert(product);
                }




                need.NeedProducts = liste;

                _uow.Needs.Insert(need);

                await _uow.Complete();


                return Ok(new
                {
                    IsSuccess = true,
                    Result = new
                    {
                        Id = need.Id,
                        Title = need.Title,
                        CreatedAt = need.CreatedAt,
                        OrderStatus = need.OrderStatus,
                        Username = username,
                        NeedProducts = liste
                    },

                    Message = "Need list created!"
                });
            }

            return Ok(new { IsSuccess = false, Result = "", Message = "Title must be filled!" });


        }


        [Route("Show-Need-For-Edit")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> ShowNeed(TakeNeedsResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;

            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId && a.Username == username && a.OrderStatus == OrderStatus.Waiting);

            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need list return null value!" });
            }

            return Ok(new { IsSuccess = true, Result = need, Message = "Need list return value!" });

        }

        [Route("Delete-Need")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> DeleteNeed(TakeNeedsResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;

            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId && a.Username == username);

            var needProduct = await _uow.NeedProducts.WhereAsync(a => a.NeedId == model.NeedId);

            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need list return null value!" });
            }

            if (needProduct != null)
            {
                foreach (var item in needProduct)
                {
                    _uow.NeedProducts.Delete(item);
                }
            }

            _uow.Needs.Delete(need);
            await _uow.Complete();

            return Ok(new { IsSuccess = true, Result = need, Message = "Need deleted!" });

        }


        [Route("Add-Product-In-Need")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> InsertProductNeed(AddProductNeed model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;


            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId && a.Username == username);

            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need list return null value!" });
            }

            if (need.NeedProducts == null)
            {
                need.NeedProducts = new List<NeedProducts>();
            }

            NeedProducts product = new NeedProducts
            {
                Id = ObjectId.GenerateNewId().ToString(),
                NeedId = model.NeedId,
                ProductDescription = model.ProductDescription


            };

            _uow.NeedProducts.Insert(product);

            var liste = _uow.NeedProducts.Where(a => a.NeedId == model.NeedId).ToList();

            liste.Add(product);

            need.NeedProducts = liste;

            _uow.Needs.Update(need);


            await _uow.Complete();

            return Ok(new { IsSuccess = true, Result = need, Message = "Need product added successfully!" });

        }


        [Route("Edit-Need-Product")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> EditNeedProduct(NeedProducts model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            if (!string.IsNullOrWhiteSpace(model.ProductDescription))
            {
                var product = await _uow.NeedProducts.FindAsync(a => a.Id == model.Id);
                product.ProductDescription = model.ProductDescription;
                _uow.NeedProducts.Update(product);

                await _uow.Complete();

                var need = _uow.Needs.Find(a => a.Id == model.NeedId);

                var liste = _uow.NeedProducts.Where(a => a.NeedId == model.NeedId).ToList();



                need.NeedProducts = liste;

                _uow.Needs.Update(need);


                await _uow.Complete();

                return Ok(new { IsSuccess = true, Result = product, Message = "Need product return updated!" });
            }

            return Ok(new { IsSuccess = false, Result = "", Message = "Need product return not updated!" });

        }

        [Route("Delete-Need-Product")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> DeleteNeedProduct(TakeNeedProductResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            if (!string.IsNullOrWhiteSpace(model.ProductId))
            {
                var username = HttpContext.User.Identity.Name;
                var product = await _uow.NeedProducts.FindAsync(a => a.Id == model.ProductId);
                if (product == null)
                {
                    return Ok(new { IsSuccess = false, Result = "", Message = "Need product return null!" });
                }

                var need = _uow.Needs.Find(a => a.Id == product.NeedId && a.Username == username);

                if (need == null)
                {
                    return Ok(new { IsSuccess = false, Result = "", Message = "Need product return null!" });
                }

                _uow.NeedProducts.Delete(product);

                await _uow.Complete();


                var liste = _uow.NeedProducts.Where(a => a.NeedId == model.NeedId).ToList();
                need.NeedProducts = liste;

                _uow.Needs.Update(need);


                await _uow.Complete();

                return Ok(new { IsSuccess = true, Result = product, Message = "Need product return deleted!" });
            }

            return Ok(new { IsSuccess = false, Result = "", Message = "Need product return not deleted!" });

        }



        [Route("Feed-For-Volunteer")]
        [Authorize(Roles = "Volunteer")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Feed(LocationViewModel volunteerLocation)
        {

            var username = HttpContext.User.Identity.Name;

            GeoCoordinate coordinate = new GeoCoordinate();
            try
            {
                coordinate = new GeoCoordinate(Convert.ToDouble(volunteerLocation.Latitude.Replace(",", ".")), Convert.ToDouble(volunteerLocation.Longitude.Replace(",", ".")));
            }
            catch (Exception ex)
            {
                coordinate = new GeoCoordinate(double.Parse(volunteerLocation.Latitude.Replace(".", ",")), double.Parse(volunteerLocation.Longitude.Replace(".", ",")));
            }

            var needsList = _uow.Needs.Where(a => a.OrderStatus == OrderStatus.Waiting && a.NeedProducts.Any() && a.Username != username).ToList();


            IList<VolunteerNeedsResponse> response = new List<VolunteerNeedsResponse>();

            if (!needsList.Any())
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need list return empty value!" });
            }


            foreach (var item in needsList)
            {
                var beneficiaryUser = await _userManager.FindByNameAsync(item.Username);
                GeoCoordinate coordinateBeneficary = new GeoCoordinate(item.Latitude,item.Longitude);
                var mesafe = coordinate.GetDistanceTo(coordinateBeneficary);

                if (mesafe > double.Parse(volunteerLocation.Range))
                {

                }else
                {
                    var data = new VolunteerNeedsResponse
                    {
                        NeedId = item.Id,
                        Title = item.Title,
                        OrderStatus = item.OrderStatus,
                        BeneficiaryNameSurname = beneficiaryUser == null ? "" : $"{beneficiaryUser.Name} {beneficiaryUser.Surname}",
                        ProfileImage = beneficiaryUser.ProfileImage,
                        CreatedAt = item.CreatedAt,
                        Location = new LocationViewModel { Latitude = item.Latitude.ToString(), Longitude = item.Longitude.ToString() },
                        Distance = GetDistance(volunteerLocation, new LocationViewModel { Latitude = item.Latitude.ToString(), Longitude = item.Longitude.ToString() })
                    };

                    response.Add(data);

                }

                
            }



            return Ok(new { IsSuccess = true, Result = response, Message = "Need list return value!" });

        }

        [Route("Beneficiary-Need-Detail")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> NeedDetail(TakeNeedsResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;


            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId);

            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need return null!" });
            }


            return Ok(new { IsSuccess = true, Result = need, Message = "Need list return value!" });

        }


        [Route("Volunteer-Feed-Need-Detail")]
        [Authorize(Roles = "Volunteer")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> VolunteerFeedNeedDetail(TakeNeedsResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;


            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId);



            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need return null!" });
            }

            if (need.NeedProducts.Count() == 0)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need not contain products! return null!" });
            }


            var beneficiaryUser = await _userManager.FindByNameAsync(need.Username);
            var needVolunteerResponse = new VolunteerNeedDetailResponse
            {
                NeedId = model.NeedId,
                NeedProducts = need.NeedProducts.ToList(),
                ProductsCount = need.NeedProducts.Count(),
                BeneficiaryNameSurname = $"{beneficiaryUser.Name} {beneficiaryUser.Surname}"
            };


            return Ok(new { IsSuccess = true, Result = needVolunteerResponse, Message = "Need list return value!" });

        }


        [Route("Accepted-Need")]
        [Authorize(Roles = "Volunteer")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> AcceptedNeed(TakeNeedsResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;


            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId && a.OrderStatus == OrderStatus.Waiting);

            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need return null!" });
            }

            need.AcceptedDate = DateTime.Now;
            need.OrderStatus = OrderStatus.Accepted;
            need.AcceptedUsername = username;

            _uow.Needs.Update(need);
            await _uow.Complete();

            var acceptedUser = await _userManager.FindByNameAsync(need.AcceptedUsername);

            return Ok(new
            {
                IsSuccess = true,
                Result = new
                {
                    Title = need.Title,
                    OrderStatus = need.OrderStatus,
                    AcceptedNameSurname = $"{acceptedUser.Name} {acceptedUser.Surname}",
                    CreatedAt = need.CreatedAt,
                    NeedProducts = need.NeedProducts,
                    AcceptedDate = need.AcceptedDate
                },
                Message = "Need list return value!"
            });

        }


        [Route("Complete-Need")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> CompleteNeed(TakeNeedsResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;


            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId && a.OrderStatus == OrderStatus.Accepted);

            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need return null!" });
            }

            need.CompletedDate = DateTime.Now;

            need.OrderStatus = OrderStatus.Completed;

            _uow.Needs.Update(need);
            var order = new Order
            {
                ID = ObjectId.GenerateNewId().ToString(),
                OrderStatus = OrderStatus.Completed,
                NeedId = need.Id,
                OrderedDate = DateTime.Now,
                Username = username,
                RequestName = need.AcceptedUsername
            };

            _uow.Order.Insert(order);
            await _uow.Complete();

            var acceptedUser = await _userManager.FindByNameAsync(need.AcceptedUsername);

            return Ok(new
            {
                IsSuccess = true,
                Result = new
                {
                    Title = need.Title,
                    OrderStatus = need.OrderStatus,
                    AcceptedNameSurname = $"{acceptedUser.Name} {acceptedUser.Surname}",
                    CreatedAt = need.CreatedAt,
                    NeedProducts = need.NeedProducts,
                    AcceptedDate = need.AcceptedDate,
                    CompletedDate = need.CompletedDate
                },
                Message = "Need list return value!"
            });

        }

        [Route("Cancel-Need")]
        [Authorize(Roles = "Beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> CancelNeed(TakeNeedsResponse model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;


            var need = await _uow.Needs.FindAsync(a => a.Id == model.NeedId && a.OrderStatus == OrderStatus.Waiting);

            if (need == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Need return null!" });
            }

            need.AcceptedDate = null;

            need.OrderStatus = OrderStatus.Waiting;
            need.AcceptedUsername = null;

            _uow.Needs.Update(need);
            await _uow.Complete();

           

            return Ok(new
            {
                IsSuccess = true,
                Result = new
                {
                    Title = need.Title,
                    OrderStatus = need.OrderStatus,                  
                    CreatedAt = need.CreatedAt,
                    NeedProducts = need.NeedProducts,
                   
                },
                Message = "Need list return value!"
            });

        }


    }
}