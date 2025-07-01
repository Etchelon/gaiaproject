using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace GaiaProject.Core.Model
{
    [Table("profiles")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("middle_name")]
        public string MiddleName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("avatar_url")]
        public string Avatar { get; set; }
        [Column("birthdate")]
        public DateTime? Birthdate { get; set; }

        public string Identifier => Id.ToString();
        public string Email { get; set; }
        public DateTime MemberSince { get; set; }
    }
}