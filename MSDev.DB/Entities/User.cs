using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSDev.DB.Entities
{
    public class User : IdentityUser
    {
        /// <summary>
        /// ���֤��
        /// </summary>
        [MaxLength(16)]
        public string IdentityCard { get; set; }

        [MaxLength(32)]
        public string RealName { get; set; }

        /// <summary>
        /// �Ա�
        /// </summary>
        [MaxLength(8)]
        public string Sex { get; set; }
        public DateTime Birthday{ get; set; }
        [MaxLength(32)]
        public string City { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// ��ϰ
        /// </summary>
        public List<UserPractice> Practice { get; set; }
    }
}
