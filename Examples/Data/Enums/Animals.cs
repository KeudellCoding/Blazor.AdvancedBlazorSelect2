using System.ComponentModel.DataAnnotations;

namespace Examples.Data.Enums {
    public enum Animals {
        [Display(Name = "Bird (Vogel)")]
        Bird,
        [Display(Name = "Horse (Pferd)")]
        Horse,
        [Display(Name = "Fish (Fisch)")]
        Fish,
        Giraffe,
        Hamster
    }
}
