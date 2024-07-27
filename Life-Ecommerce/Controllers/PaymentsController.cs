using Application.Services.Order;
using Application.Services.Payment;
using Domain.DTOs.Payment;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;


namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _config;

        public PaymentsController(IPaymentService paymentService, IOrderService orderService, ILogger<PaymentsController> logger, IConfiguration config)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _logger = logger;
            _config = config;
        }

        [HttpGet("payments")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            var payments = await _paymentService.GetPayments();
            return Ok(payments);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return Ok(payment);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetPaymentById(paymentId);
            return Ok(payment);
        }



        [HttpGet("payments-per-month")]
        public async Task<IActionResult> GetPaymentsPerMonth()
        {
            var monthlyPayments = await _paymentService.GetPaymentsPerMonth();
            return Ok(monthlyPayments);
        }

        //[HttpPost("create-cash-payment")]
        //public async Task<IActionResult> CreateCashPayment([FromBody] CreateCashPaymentDto createCashPaymentDto)
        //{
        //    //try
        //    //{
        //        // Retrieve the order details from the OrderService
        //        var order = await _orderService.GetOrderById(createCashPaymentDto.OrderId);
        //        if (order == null)
        //        {
        //            return NotFound("Order not found");
        //        }

        //        var payment = new Payment
        //        {
        //            PaymentDate = createCashPaymentDto.PaymentDate,
        //            Amount = createCashPaymentDto.Amount,
        //            PaymentStatus = "Completed", 
        //            PaymentMethod = "Cash",
        //            OrderId = createCashPaymentDto.OrderId,
        //            TransactionId = "null"
        //        };

        //        _paymentService.Create(payment);
        //        await _paymentService.SaveChangesAsync();

        //        return Ok(payment);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    _logger.LogError(ex, "Error creating cash payment: " + ex.Message);
        //    //    return StatusCode(500, "Error creating cash payment");
        //    //}
        //}


        [HttpPost("create-cash-payment")]
        public async Task<IActionResult> CreateCashPayment([FromBody] CreateCashPaymentDto createPaymentDto)
        {
            //try
            //{
                var payment = await _paymentService.CreateCashPayment(createPaymentDto);
                return Ok(payment);
            //}
            //catch (Exception ex)
            //{
                //_logger.LogError(ex, "Error creating cash payment: " + ex.Message);
                //return StatusCode(500, "Error creating cash payment");
           // }
        }

        [HttpGet("transaction/{transactionId}")]
        public async Task<IActionResult> GetPaymentByTransactionId(string transactionId)
        {
            var payment = await _paymentService.GetPaymentByTransactionId(transactionId);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }


        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            try
            {
                StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

                var order = await _orderService.GetOrderById(createPaymentDto.OrderId);
                if (order == null)
                {
                    return NotFound("Order not found");
                }

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "eur",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = createPaymentDto.Description,
                                },
                                UnitAmount = (long)(order.OrderTotal * 100), // Convert to cents
                            },
                            Quantity = 1,
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = _config["Urls:SuccessUrl"],
                    CancelUrl = _config["Urls:CancelUrl"],
                    BillingAddressCollection = "required",
                };

                var service = new Stripe.Checkout.SessionService();
                var session = await service.CreateAsync(options);

                var payment = new Payment
                {
                    PaymentDate = DateTime.UtcNow,
                    Amount = order.OrderTotal,
                    PaymentStatus = "Pending",
                    PaymentMethod = "card",
                    TransactionId = session.Id,
                    OrderId = createPaymentDto.OrderId
                };

                _paymentService.Create(payment);
                await _paymentService.SaveChangesAsync();

                return Ok(new { url = session.Url });
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe error creating payment session: " + stripeEx.Message);
                return StatusCode(500, "Stripe error creating payment session");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment session: " + ex.Message);
                return StatusCode(500, "Error creating payment session");
            }
        }




        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeSign = Request.Headers["Stripe-Signature"];
                var webhookSecret = _config["Stripe:WebhookKey"];

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSign,
                    webhookSecret,
                    throwOnApiVersionMismatch: false,
                    tolerance: 800
                );

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    var payment = await _paymentService.GetPaymentByTransactionId(session.Id);

                    if (payment != null)
                    {
                        payment.PaymentStatus = "Completed";
                        await _paymentService.SaveChangesAsync();
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing Stripe webhook");
                return BadRequest();
            }
        }
    }
}
