namespace ClientApi.Models;

public class SavedNumber {
    public int SavedNumberId { get; set; }
    public int Value { get; set; }
    public bool IsBanned { get; set; } = default;
}