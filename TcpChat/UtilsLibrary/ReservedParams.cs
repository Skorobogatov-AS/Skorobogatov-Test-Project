namespace UtilsLibrary
{
    /// <summary>
    /// Зарезервированные смещения символов для сообщений. 
    /// </summary>
    public static class ReservedParams
    {
        /// <summary>
        /// Таймаут отключения бездействующего пользователя в миллисекундах.
        /// </summary>
        public const int USER_TIMEOUT = 60000 * 3;

        /// <summary>
        /// Максимальная разрядность для количества пользователей.
        /// Количество символов зарезервированное под id пользователя.
        /// </summary>
        public const int  USERS_DIGIT_CAPACITY = 4;
    }
}
