namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Xml.Serialization;
    using System.IO;
    using System.Globalization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2:0.00}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";



        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportMoviesDto[]>(jsonString);

            List<Movie> movies = new List<Movie>();

            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {

                var checkGenre = Enum.TryParse(dto.Genre, out Genre genre);
                var checkTimespan = TimeSpan.TryParse(dto.Duration, out TimeSpan duration);


                //Check condition works and catches invalid
                if (!IsValid(dto) || !checkGenre || !checkTimespan)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Movie movie = new Movie
                {
                    Title = dto.Title,
                    Genre = genre,
                    Duration = duration,
                    Rating = dto.Rating,
                    Director = dto.Director

                };

                movies.Add(movie);

                sb.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, dto.Genre, movie.Rating));
            }

            context.Movies.AddRange(movies);
            context.SaveChanges();


            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportHallDto[]>(jsonString);

            List<Hall> halls = new List<Hall>();

            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Hall hall = new Hall
                {
                    Name = dto.Name,
                    Is4Dx = dto.Is4Dx,
                    Is3D = dto.Is3D,
                };

                ICollection<Seat> seats = CreateSeats(context, hall, dto.Seats);

                hall.Seats = seats;

                halls.Add(hall);


                string typeOfProjection = string.Empty;

                if (hall.Is3D && hall.Is4Dx)
                {
                    typeOfProjection = "4Dx/3D";
                }
                else if (hall.Is3D)
                {
                    typeOfProjection = "3D";
                }
                else if (hall.Is4Dx)
                {
                    typeOfProjection = "4Dx";
                }
                else
                {
                    typeOfProjection = "Normal";

                }

                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hall.Name, typeOfProjection, hall.Seats.Count));
            }

            context.Halls.AddRange(halls);
            context.SaveChanges();


            string result = sb.ToString().TrimEnd();

            return result;
        }


        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));

            var projectionsDto = (ImportProjectionDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            List<Projection> projections = new List<Projection>();

            var sb = new StringBuilder();

            foreach (var dto in projectionsDto)
            {
                var hall = GetHall(context, dto.HallId);
                var movie = GetMovie(context, dto.MovieId);

                if (hall == null || movie == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Projection projection = new Projection
                {
                    HallId = dto.HallId,
                    MovieId = dto.MovieId,
                    DateTime = DateTime.ParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };

                projections.Add(projection);

                sb.AppendLine(string.Format(SuccessfulImportProjection, movie.Title, projection.DateTime.ToString("MM/dd/yyyy")));
            }

            context.AddRange(projections);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerTicketDto[]), new XmlRootAttribute("Customers"));

            var customerDtos = (ImportCustomerTicketDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            List<Customer> customers = new List<Customer>();

            var sb = new StringBuilder();

            foreach (var dto in customerDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool invalidTicket = false;


                ICollection<Ticket> tickets = new HashSet<Ticket>();

                HashSet<int> projectionIds = context.Projections.Select(x => x.Id).ToHashSet();

                Customer customer = new Customer
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    Balance = dto.Balance
                };


                foreach (var ticket in dto.Tickets)
                {
                    if (!projectionIds.Contains(ticket.ProjectionId))
                    {
                        invalidTicket = true;
                        break;
                    }

                    tickets.Add(new Ticket
                    {
                        ProjectionId = ticket.ProjectionId,
                        Customer = customer,
                        Price = ticket.Price
                    });
                }

                if (invalidTicket)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                customer.Tickets = tickets;

                customers.Add(customer);

                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();

            return result;
        }


        private static Hall GetHall(CinemaContext context, int hallId)
        {
            var hall = context.Halls.FirstOrDefault(x => x.Id == hallId);

            return hall;
        }

        private static Movie GetMovie(CinemaContext context, int movieId)
        {
            var movie = context.Movies.FirstOrDefault(x => x.Id == movieId);

            return movie;
        }

        private static ICollection<Seat> CreateSeats(CinemaContext context, Hall hall, int seats)
        {
            ICollection<Seat> seatsCollection = new HashSet<Seat>();

            for (int i = 0; i < seats; i++)
            {
                seatsCollection.Add(new Seat
                {
                    Hall = hall,
                });
            }

            return seatsCollection;
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }

    }
}