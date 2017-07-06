using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.ViewModels;

namespace WFP.ICT.Web.Controllers
{
    public class TestController : BaseController
    {
        public async Task<ActionResult> Edit(Guid? id)
        {
            IEnumerable<Address> addressEntities = db.Addresses.ToList();
            var viewModel = new PersonEditViewModel
            {
                Addresses = addressEntities.Select(a => new AddressEditorViewModel
                {
                    Id = a.Id,
                    Street = a.Address1,
                    City = a.Suburb
                }),
            };

            return View(viewModel);
        }

        public ActionResult GetAddressEditor()
        {
            return PartialView("Editors/_AddressEditor", new AddressEditorViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Guid? id, PersonEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save changes of viewModel.Addresses to database

                return RedirectToAction("Details", new { id = id });
            }
            else
                return View(viewModel);
        }

        public ActionResult Details(Guid? id)
        {
            IEnumerable<Address> addressEntities = db.Addresses.ToList();
            var viewModel = new PersonEditViewModel
            {
                Addresses = addressEntities.Select(a => new AddressEditorViewModel
                {
                    Id = a.Id,
                    Street = a.Address1,
                    City = a.Suburb
                }),
            };
            return View(viewModel);
        }
    }
}