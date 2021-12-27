namespace BlazingPizza
{
    public class OrderState
    {
        public bool IsShowingPizzaDialog { get; set; }

        public Pizza Pizza { get; set; }

        public Order Order { get; set; } = new Order();

        public void ShowingPizzaDialog(PizzaSpecial special)
        {
            Pizza = new Pizza
            {
                Special = special,
                SpecialId = special.Id,
                Size = Pizza.DefaultSize,
                Toppings = new()
            };

            IsShowingPizzaDialog = true;
        }

        public void CancelPizzaDialog()
        {
            Pizza = null;
            IsShowingPizzaDialog = false;
        }

        public void ConfirmPizzaDialog()
        {
            Order.Pizzas.Add(Pizza);
            Pizza = null;
            IsShowingPizzaDialog = false;
        }

        public void RemovePizza(Pizza pizza)
        {
            Order.Pizzas.Remove(pizza);
        }

        public void ResetOrder()
        {
            Order = new();
        }
    }
}