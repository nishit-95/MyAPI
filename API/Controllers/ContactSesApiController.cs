using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Models;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactSesApiController : ControllerBase
    {
        private readonly IContactInterface _contact;
        public ContactSesApiController(IConfiguration configuration, IContactInterface contact)
        {
            _contact = contact;
        }


        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            string userid = HttpContext.Request.Query["userid"].ToString();
            List<t_Contact> list;
            if (userid != "")
            {
                list = await _contact.GetAllByUser(userid);
            }
            else
            {
                list = await _contact.GetAll();
            }
            return Ok(list);
        }


        [HttpGet("GetContactById/{id}")]
        public async Task<IActionResult> GetContactById(string id)
        {
            var contact = await _contact.GetOne(id);
            if (contact == null)
                return BadRequest(new { success = false, message = "There was no contact found" });
            return Ok(contact);
        }

        [HttpPost("AddContact")]
        public async Task<IActionResult> AddContact([FromForm] t_Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }
            if (contact.ContactPicture != null && contact.ContactPicture.Length > 0)
            {
                // Save the uploaded file
                var fileName = contact.c_Email + Path.GetExtension(contact.ContactPicture.FileName);
                var filePath = Path.Combine("../MVC/wwwroot/contact_images", fileName);
                contact.c_Image = fileName;
                System.IO.File.Delete(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    contact.ContactPicture.CopyTo(stream);
                }
            }
            var status = await _contact.Add(contact);
            if (status == 1)
            {
                return Ok(new { success = true, message = "contact Insterted Successfully!!!!!" });
            }
            else
            {
                return BadRequest(new { success = false, message = "There was some error while adding the contact" });
            }
        }

        // [HttpPut("UpdateContact")]
        [HttpPut]
        // [Route("UpdateContact")]
        public async Task<IActionResult> UpdateContact([FromForm] t_Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }
            if (contact.ContactPicture != null && contact.ContactPicture.Length > 0)
            {
                var fileName = contact.c_Email + Path.GetExtension(contact.ContactPicture.FileName);
                var filePath = Path.Combine("../MVC/wwwroot/contact_images", fileName);
                contact.c_Image = fileName;
                System.IO.File.Delete(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    contact.ContactPicture.CopyTo(stream);
                }
            }
            int status = await _contact.Update(contact);
            if (status == 1)
            {
                return Ok(new { success = true, message = "Contact Updated Successfully" });
            }
            else
            {
                return BadRequest(new { success = false, message = "There was some error while Update Contact" });
            }
        }
        [HttpDelete("DeleteContact/{id}")]

        public async Task<IActionResult> DeleteContact(string id)
        {
            int status = await _contact.Delete(id);
            if (status == 1)
            {
                return Ok(new { success = true, message = "Contact Deleted Successfully" });
            }
            else
            {
                return BadRequest(new { success = false, message = "There Is Some Error while Delete Contact" });
            }
        }
    }
}