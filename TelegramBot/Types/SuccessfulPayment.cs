namespace TelegramBot.Types
{
    public class SuccessfulPayment
    {
        public string currency;
        public long total_amount;
        public string invoice_payload;
        public OrderInfo order_info;
        public string telegram_payment_charge_id;
        public string provider_payment_charge_id;
    }
}