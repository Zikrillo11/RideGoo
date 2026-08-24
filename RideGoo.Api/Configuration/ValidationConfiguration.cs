using RideGoo.Api.Filters;

namespace RideGoo.Api.Configuration;

public static class ValidationConfiguration
{
    public static IMvcBuilder AddValidationConfiguration(this IMvcBuilder builder)
    {
        // ValidationFilter allaqachon Controllers bilan birga ro'yxatdan o'tkaziladi,
        // bu metod kelajakda qo'shimcha global filterlar qo'shish uchun joy bo'lib xizmat qiladi
        return builder;
    }
}