using CarWashService.Web.Models.Entities;
using CarWashService.Web.Models.Entities.Serialized;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace CarWashService.Web.Controllers
{
    public class BranchesController : ApiController
    {
        private readonly CarWashBaseEntities db = new CarWashBaseEntities();

        // GET: api/Branches
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public IHttpActionResult GetBranch()
        {
            List<SerializedBranch> branches = db.Branch
                .ToList()
                .ConvertAll(b => new SerializedBranch(b));
            return Ok(branches);
        }

        // GET: api/Branches/5
        [ResponseType(typeof(Branch))]
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public async Task<IHttpActionResult> GetBranch(int id)
        {
            Branch branch = await db.Branch.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            return Ok(new SerializedBranch(branch));
        }

        // GET: api/Branches/contacts?id=5
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        [Route("api/branches/contacts")]
        public async Task<IHttpActionResult> GetBranchContacts(int id)
        {
            Branch branch = await db.Branch.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            return Ok(branch.BranchPhone
                .Select(p => p.PhoneNumber));
        }

        // PUT: api/Branches/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = "Администратор")]
        public async Task<IHttpActionResult> PutBranch(int id, SerializedBranch serializedBranch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != serializedBranch.Id)
            {
                return BadRequest();
            }

            City city = await db.City
              .FirstOrDefaultAsync(c => c.Name == serializedBranch.CityName);
            if (city == null)
            {
                city = new City
                {
                    Name = serializedBranch.CityName
                };
                _ = db.City.Add(city);
            }

            Address address = await db.Address
                .FirstOrDefaultAsync(b => b.StreetName == serializedBranch.StreetName);
            if (address == null)
            {
                address = new Address
                {
                    City = city,
                    StreetName = serializedBranch.StreetName,
                };
            }

            Branch branchToPut = new Branch
            {
                Id = serializedBranch.Id,
                Title = serializedBranch.Title,
                AddressId = address.Id,
                WorkFrom = TimeSpan.Parse(serializedBranch.WorkFrom),
                WorkTo = TimeSpan.Parse(serializedBranch.WorkTo)
            };

            Branch branchFromDb = db.Branch.Find(serializedBranch.Id);

            db.Entry(branchFromDb)
                .CurrentValues
                .SetValues(branchToPut);

            try
            {
                _ = await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Branches
        [ResponseType(typeof(Branch))]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IHttpActionResult> PostBranch(
            SerializedBranch serializedBranch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            City city = await db.City
                .FirstOrDefaultAsync(c => c.Name == serializedBranch.CityName);
            if (city == null)
            {
                city = new City
                {
                    Name = serializedBranch.CityName
                };
                _ = db.City.Add(city);
            }

            Address address = await db.Address
                .FirstOrDefaultAsync(b => b.StreetName == serializedBranch.StreetName);
            if (address == null)
            {
                address = new Address
                {
                    City = city,
                    StreetName = serializedBranch.StreetName,
                };
            }
            if (serializedBranch.Id == 0)
            {

                Branch branchToAdd = new Branch
                {
                    Title = serializedBranch.Title,
                    Address = address,
                    WorkFrom = TimeSpan.Parse(serializedBranch.WorkFrom),
                    WorkTo = TimeSpan.Parse(serializedBranch.WorkTo)
                };
                foreach (string phone in serializedBranch.PhoneNumbers)
                {
                    branchToAdd.BranchPhone.Add(new BranchPhone
                    {
                        PhoneNumber = phone
                    });
                }
                _ = db.Branch.Add(branchToAdd);
                _ = await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi",
                                      new { id = branchToAdd.Id },
                                      branchToAdd.Id);
            }
            else
            {
                Branch branch = await db.Branch.FindAsync(serializedBranch.Id);
                if (branch == null)
                {
                    return NotFound();
                }
                db.Branch.Find(serializedBranch.Id).Title = serializedBranch.Title;
                db.Branch.Find(serializedBranch.Id).Address = address;
                db.Branch.Find(serializedBranch.Id).WorkFrom = TimeSpan.Parse(serializedBranch.WorkFrom);
                db.Branch.Find(serializedBranch.Id).WorkTo = TimeSpan.Parse(serializedBranch.WorkTo);
                foreach (string phone in serializedBranch.PhoneNumbers)
                {
                    if (branch.BranchPhone.Any(p => p.PhoneNumber == phone))
                    {
                        continue;
                    }
                    BranchPhone newPhone = new BranchPhone
                    {
                        PhoneNumber = phone,
                    };
                    branch.BranchPhone.Add(newPhone);
                }

                _ = await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi",
                                      new { id = branch.Id },
                                      serializedBranch.Id);
            }
        }

        // DELETE: api/Branches/5
        [ResponseType(typeof(Branch))]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IHttpActionResult> DeleteBranch(int id)
        {
            Branch branch = await db.Branch.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            _ = db.Branch.Remove(branch);
            _ = await db.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BranchExists(int id)
        {
            return db.Branch.Count(e => e.Id == id) > 0;
        }

        [Authorize(Roles = "Администратор")]
        [Route("api/branches/{branchId}/add/service")]
        [HttpPost]
        public async Task<IHttpActionResult> AssignService(int branchId, int serviceId)
        {
            Branch branch = await db.Branch.FindAsync(branchId);
            if (branch == null)
            {
                return NotFound();
            }

            Service service = await db.Service.FindAsync(serviceId);

            if (service == null)
            {
                return NotFound();
            }
            if (branch.Service.Any(s => s.Id == service.Id))
            {
                return Conflict();
            }

            branch.Service.Add(service);
            _ = await db.SaveChangesAsync();

            return Ok(new
            {
                BranchId = branchId,
                ServiceId = serviceId
            });
        }
        [Authorize(Roles = "Администратор")]
        [Route("api/branches/{branchId}/add/phones")]
        [HttpPost]
        public async Task<IHttpActionResult> AssignPhone(int branchId,
                                                         List<string> phones)
        {
            Branch branch = await db.Branch.FindAsync(branchId);
            if (branch == null)
            {
                return NotFound();
            }

            foreach (string phone in phones)
            {
                BranchPhone phoneFromDb = await db.BranchPhone
                    .FirstOrDefaultAsync(p => p.PhoneNumber == phone);
                if (phoneFromDb != null)
                {
                    continue;
                }
                BranchPhone newPhone = new BranchPhone
                {
                    PhoneNumber = phone,
                    BranchId = branchId
                };
                _ = db.BranchPhone.Add(newPhone);
            }

            _ = await db.SaveChangesAsync();

            return Ok();
        }
    }
}