﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
            Purchases = new HashSet<Purchase>();
        }

        public int Id { get; set; }

        [Required]
        [RegularExpression("^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$")]
        public string Number { get; set; }

        [Required]
        [RegularExpression("^[0-9]{3}$")]
        public string Cvc { get; set; }

        public CardType Type { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }

    public enum CardType
    {
        Debit,
        Credit
    }
}