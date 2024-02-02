using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System.Collections.Generic;

namespace PriceSignageSystem.Models.Repository
{
    public class EditReasonRepository : IEditReasonRepository
    {
        private readonly ApplicationDbContext _db;

        public EditReasonRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<EditReason> GetAllReasons()
        {
            var data = _db.EditReasons;
            return data;
        }
    }
}