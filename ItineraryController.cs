using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Connxys.LuggageTag.Core_New.Data;
using Connxys.LuggageTag.Core_New.Repositories;
using Connxys.LuggageTag.Core_New.Repositories.Interfaces;
using Connxys.LuggageTag.Services.DTOs;
using Connxys.LuggageTag.Services.WCFImplementations;
using Connxys.LuggageTag.Web.Models;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace Connxys.LuggageTag.Web.Controllers
{
    /// <summary>
    /// The Itinerary controller
    /// </summary>
    public class ItineraryController : Controller
    {
        private static List<Flights> _listF = new List<Flights>();
        private static List<BasicItineraryModel> _listIti = new List<BasicItineraryModel>();
        private static List<BagTagDesignation> _listB = new List<BagTagDesignation>();
        private IAirportsRepository _airportsRepository;
        private IAirlinesRepository _airlinesRepository;
        private IItinerarysRepository _itinerarysRepository;
        private IPassengerRepository _passengerRepository;
        private IFlightsRepository _flightsRepository;
        private IUsersRepository _usersRepository;
        private IBagTagDesignationRepository _bagTagDesignationRepository;
        private IMonitoredFlightsRepository _monitoredFlightsRepository;
        private IStoreProcedureRepository _storeProcedureRepository;
        private ITagsRepository _tagsRepository;
        private IItineraryPingersRepository _itineraryPingersRepository;

      
        [Authorize]
        public ActionResult Index()
        {
            _itinerarysRepository = new ItinerarysRepository();

            if (TempData["messageDelete"] != null)
                ViewBag.Message = TempData["messageDelete"].ToString();
            return View();
        }

        /// <summary>
        /// Add bag by model information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddBag(TestaddBagTagDesignationModel model)
        {
                var btd = new BagTagDesignation {
                    ConnxysTagID = model.BagTagDesignation,
                    BagIATANumber = model.BagTagNumber.ToString()
                };

                _listB.Add(btd);
                ViewBag.UserList = _listB;
                return Json(_listB.ToList(), JsonRequestBehavior.AllowGet); 
        }

        /// <summary>
        /// Remove bag by index number
        /// </summary>
        /// <param name="index"></param>
        [HttpGet]
        public void RemoveBag(int index)
        {
            _listB.RemoveAt(index);
            ViewBag.UserList = _listB;
        }

        /// <summary>
        /// Insert Basic Itinerary Information
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult InsertBasic()
        {
            ViewBag.ConnxysTagId = GetConnxysTagsIds();

            _listF = new List<Flights>();
            _listB = new List<BagTagDesignation>();
            PopulateList();

            var defaultValue = new BasicItineraryModel
            {
                DepartDate = DateTime.UtcNow,
                ArrivalDate = DateTime.UtcNow,
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow,
                DepartureMinute = DateTime.UtcNow.Minute,
                ArrivalMinute = DateTime.UtcNow.Minute,
                DepartureSecond = DateTime.UtcNow.Second,
                ArrivalSecond = DateTime.UtcNow.Second
            };


            defaultValue.DepartureHour = defaultValue.DepartureTime.Value.AddHours(1).Hour;
            defaultValue.ArrivalHour = defaultValue.ArrivalTime.Value.AddHours(2).Hour;
            
            if (defaultValue.DepartureHour < DateTime.UtcNow.Hour)
            {
                defaultValue.DepartDate = defaultValue.DepartDate.Value.AddDays(1);
                defaultValue.DepartureTime = defaultValue.DepartureTime.Value.AddHours(1);
            }
            else 
            {
                defaultValue.DepartureTime = defaultValue.DepartureTime.Value.AddHours(1);
            }
            if (defaultValue.ArrivalHour < DateTime.UtcNow.Hour)
            {
                defaultValue.ArrivalDate = defaultValue.ArrivalDate.Value.AddDays(1);
                defaultValue.ArrivalTime = defaultValue.ArrivalTime.Value.AddHours(2);
            }
            else {
                defaultValue.ArrivalTime = defaultValue.ArrivalTime.Value.AddHours(2);
            }
            ViewBag.DepartDate = Convert.ToDateTime(defaultValue.DepartureTime).ToString("MM/dd/yyyy HH:mm");
            ViewBag.ArrivalDate = Convert.ToDateTime(defaultValue.ArrivalTime).ToString("MM/dd/yyyy HH:mm");
            return View(defaultValue);
        }
        
        /// <summary>
        /// Validate flight information with Flight Status server, this is important to validate if the information is true and this is correct.
        /// </summary>
        /// <param name="AirlineName"></param>
        /// <param name="FlightNumber"></param>
        /// <param name="DepartureName"></param>
        /// <param name="ArrivalName"></param>
        /// <param name="DepartureTime"></param>
        /// <param name="ArrivalTime"></param>
        /// <param name="Stops"></param>
        /// <param name="TripTypeName"></param>
        /// <param name="PassengerName"></param>
        /// <param name="PassengerLastName"></param>
        /// <param name="PassengerEmail"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult FlightValidation(string AirlineName, string FlightNumber, string DepartureName, string ArrivalName, string DepartureTime, string ArrivalTime, string Stops, string TripTypeName, string PassengerName, string PassengerLastName, string PassengerEmail) 
        {
            var flightService = new ConnxysWCFRestService();
            var fsm = new FlightStatusMessages();
            string jsonData;
            GoogleTimeZone timezone = new GoogleTimeZone();

            fsm.isValid = false;
            fsm.messages = new Message();
            
            _airlinesRepository = new AirlinesRepository();
            _tagsRepository = new TagsRepository();
            _airportsRepository = new AirportsRepository();

            if (!AirlineName.Equals("") && !FlightNumber.Equals("") && !DepartureName.Equals("") && !ArrivalName.Equals("") &&
                !DepartureTime.Equals("") && !ArrivalTime.Equals("") && !Stops.Equals("") && !TripTypeName.Equals(""))
            {
                #region Complete Info
                var model = new BasicItineraryModel
                {
                    AirlineName = AirlineName,
                    FlightNumber = Convert.ToInt32(FlightNumber),
                    DepartureName = DepartureName,
                    ArrivalName = ArrivalName,
                    DepartureTime = Convert.ToDateTime(DepartureTime),
                    ArrivalTime = Convert.ToDateTime(ArrivalTime),
                    Stops = Convert.ToInt32(Stops),
                    TripTypeName = TripTypeName,
                    PassengerName = PassengerName,
                    PassengerLastName = PassengerLastName,
                    PassengerEmail = PassengerEmail
                };


                //Get Airport Information: Departure and Arrival airports
                Airports DepartureAirport = new Airports();
                DepartureAirport = _airportsRepository.GetAirportbyName(DepartureName);
                Airports ArrivalAirport = new Airports();
                ArrivalAirport = _airportsRepository.GetAirportbyName(ArrivalName);

                //Get UTC of Departure Airport
                Double UTCDepartureAir = timezone.GetLocalDateTime(DepartureAirport.Latitude, DepartureAirport.Longitude, DateTime.UtcNow);

                //Get UTC of Arrival Airport
                Double UTCArrivalAir = timezone.GetLocalDateTime(ArrivalAirport.Latitude, ArrivalAirport.Longitude, DateTime.UtcNow);

                //Get the difference between UTC and local time to Departure and Arrival airport
                model.DepartDateTimeUTC = model.DepartureTime.Value.AddHours(UTCDepartureAir);
                model.ArrivalDateTimeUTC = model.ArrivalTime.Value.AddHours(UTCArrivalAir);

                var tempAirline = _airlinesRepository.GetAirlinebyName(model.AirlineName);
                var result = flightService.GetInfoFlightStatus(model.FlightNumber, tempAirline.AirlineDesignator, Convert.ToDateTime(model.DepartDateTimeUTC));
                var listResult = result.flightStatuses.ToList();
                var totalFlightStatus = listResult.Count;
                
                
                //Time validation validate the hour and minutes selected by the user                
                if (model.DepartureTime.Value >= model.ArrivalTime.Value)
                {
                    fsm.messages.mainMessage = "Arrival Time must be greater than Departure Time";
                    jsonData = new JavaScriptSerializer().Serialize(fsm);
                    return Json(jsonData, JsonRequestBehavior.AllowGet); 
                }

                var utcNow = DateTime.UtcNow;
                if (model.DepartureTime.Value < utcNow || model.ArrivalTime.Value < utcNow)
                {
                    fsm.messages.mainMessage = "Departure Time and Arrival Time must be greater or equal than Current UTC time";
                    jsonData = new JavaScriptSerializer().Serialize(fsm);
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }

                if (totalFlightStatus > 0)
                {
                    /* If is a real flight */
                    foreach (var item in listResult)
                    {
                        fsm.messages = new Message();
                        var matchesElements = true;
                        var flightStatus = item;

                        var serviceDepartureDate = flightStatus.departureDate.dateUtc.ToUniversalTime();//DateTimeOffset.Parse(flightStatus.departureDate.dateUtc).UtcDateTime;
                        var serviceArrivalDate = flightStatus.arrivalDate.dateUtc.ToUniversalTime();//DateTimeOffset.Parse().UtcDateTime;
                        
                        #region Departure and Arrival Airports match with real flight
                        if (model.DepartureName == flightStatus.departureAirportFsCode && model.ArrivalName == flightStatus.arrivalAirportFsCode)
                        {
                            if (serviceDepartureDate.CompareTo(model.DepartDateTimeUTC) != 0)
                            {
                                fsm.messages.depDateMessage = "The Departure Date doesn't match with real flight.";
                                matchesElements = false;
                            }
                            if (serviceArrivalDate.CompareTo(model.ArrivalDateTimeUTC) != 0)
                            {
                                fsm.messages.arrDateMessage = "The Arrival Date doesn't match with real flight.";
                                matchesElements = false;
                            }
                            if (matchesElements)
                                fsm.isValid = true;

                            break;
                        }
                        #endregion

                        #region Departure Airport match with real flight
                        if (model.DepartureName == flightStatus.departureAirportFsCode)
                        {
                            if (model.ArrivalName != flightStatus.arrivalAirportFsCode)
                            {
                                fsm.messages.arrivalMessage = "The Arrival doesn't match with real flight.";
                            }
                            if (serviceDepartureDate.CompareTo(model.DepartDateTimeUTC) != 0)
                            {
                                fsm.messages.departureMessage = "The Departure Date doesn't match with real flight.";
                            }
                            if (serviceArrivalDate.CompareTo(model.ArrivalDateTimeUTC) != 0)
                            {
                                fsm.messages.arrDateMessage = "The Arrival Date doesn't match with real flight.";
                            }
                            break;
                        }
                        #endregion

                        #region Arrival Airport match with real flight
                        if (model.ArrivalName == flightStatus.arrivalAirportFsCode)
                        {
                            if (model.DepartureName != flightStatus.departureAirportFsCode)
                            {
                                fsm.messages.departureMessage = "The Departure doesn't match with real flight.";
                            }
                            if (serviceDepartureDate.CompareTo(model.DepartDateTimeUTC) != 0)
                            {
                                fsm.messages.depDateMessage = "The Departure Date doesn't match with real flight.";
                            }
                            if (serviceArrivalDate.CompareTo(model.ArrivalDateTimeUTC) != 0)
                            {
                                fsm.messages.arrDateMessage = "The Arrival Date doesn't match with real flight.";
                            }
                            break;
                        }
                        #endregion

                        #region Departure and Arrival Airports don't match with real flight

                        if (model.DepartureName == flightStatus.departureAirportFsCode || model.ArrivalName == flightStatus.arrivalAirportFsCode)
                            continue;

                        fsm.messages.departureMessage = "The Departure doesn't match with real flight.";
                        fsm.messages.arrivalMessage = "The Arrival doesn't match with real flight.";

                        if (serviceDepartureDate.CompareTo(model.DepartDateTimeUTC) != 0)
                        {
                            fsm.messages.depDateMessage = "The Departure Date doesn't match with real flight.";
                        }
                        if (serviceArrivalDate.CompareTo(model.ArrivalDateTimeUTC) != 0)
                        {
                            fsm.messages.arrDateMessage = "The Arrival Date doesn't match with real flight.";
                        }

                        #endregion
                        
                    }
                }
                else
                {
                    /* If is not a real flight */
                    fsm.isValid = true;
                }

                if (fsm.isValid)
                {
                    // Insert the flight into flight's list
                    InsertFlightIntoList(model);
                }
                else {
                    fsm.messages.mainMessage = "Error: Flight number " + model.FlightNumber + " doesn't match the Flight Stats information. Please edit the information below.";
                }

                #endregion
            }
            else{
                fsm.messages.mainMessage = "Information required: Please complete all the flight information fields.";
            }

            jsonData = new JavaScriptSerializer().Serialize(fsm);
            return Json(jsonData, JsonRequestBehavior.AllowGet); 
        }


        /// <summary>
        /// Insert flight into a temporal list for then save the flight information in the database of fights and itineraries.
        /// </summary>
        /// <param name="model"></param>
        public void InsertFlightIntoList(BasicItineraryModel model) 
        {
            var airlineInfo = _airlinesRepository.GetAirlinebyName(model.AirlineName);
            _airportsRepository = new AirportsRepository();
            _passengerRepository = new PassengerRepository();

            if (airlineInfo == null)
                return;

            var airlineId = airlineInfo.AirlineID;
            var airportInfo1 = _airportsRepository.GetAirportbyName(model.DepartureName);
            var airportInfo2 = _airportsRepository.GetAirportbyName(model.ArrivalName);

            if ((airportInfo1 == null) || (airportInfo2 == null))
                return;

            var departureLocation = airportInfo1.AirportID;
            var arrivalLocation = airportInfo2.AirportID;

            var flight = new Flights
            {
                AirlineID = airlineId,
                DepartureLocation = departureLocation,
                FlightNumber = model.FlightNumber,
                ArrivalLocation = arrivalLocation,
                DepartureDateTime = (DateTime) model.DepartureTime,
                ArrivalDateTime = (DateTime) model.ArrivalTime,
                
            };

            _listF.Add(flight);
            _listIti.Add(model);
        

            var passenger = _passengerRepository.Get(model.PassengerEmail);
            if (passenger == null)
            {
                passenger = new Passengers
                {
                    PassengerEmail = model.PassengerEmail,
                    PassengerLastName = model.PassengerLastName,
                    PassengerName = model.PassengerName,
                    TokenId = "TokenID"
                };

                _passengerRepository.Add(passenger);
            }
        }

        /// <summary>
        /// Insert itineary and flights in the database.
        /// </summary>
        /// <param name="Stops"></param>
        /// <param name="TripTypeName"></param>
        /// <param name="PassengerName"></param>
        /// <param name="PassengerLastName"></param>
        /// <param name="PassengerEmail"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult InsertBasic(string Stops, string TripTypeName, string PassengerName, string PassengerLastName, string PassengerEmail)
        {           
            _airlinesRepository = new AirlinesRepository();
            _airportsRepository = new AirportsRepository();
            _tagsRepository = new TagsRepository();
            _passengerRepository = new PassengerRepository();
            _itineraryPingersRepository = new ItineraryPingersRepository();
            new UserTagsRepository();
            _usersRepository = new UsersRepository();
            _monitoredFlightsRepository = new MonitoredFlightsRepository();
            _itinerarysRepository = new ItinerarysRepository();
            _flightsRepository = new FlightsRepository();   
            new GatewayAirlineRepository();

            #region Create Itinerary   

            var model = new BasicItineraryModel
            {
                //ConnxysTagId = ConnxysTagId,
                DepartureTime = _listF.First().DepartureDateTime,
                ArrivalTime = _listF.Last().ArrivalDateTime,
                DepartureName = _airportsRepository.Get(_listF.First().DepartureLocation).AirportDesignationLetters,
                ArrivalName = _airportsRepository.Get(_listF.Last().ArrivalLocation).AirportDesignationLetters,
                Stops = Convert.ToInt32(Stops),
                TripTypeName = TripTypeName,
                DepartDateTimeUTC = _listIti.First().DepartDateTimeUTC,
                ArrivalDateTimeUTC = _listIti.Last().ArrivalDateTimeUTC
                
                
            };
            
            object jsonData;
            //Get Passenger Information
            var passenger = _passengerRepository.Get(PassengerEmail);
            passenger.PassengerName = PassengerName;
            passenger.PassengerLastName = PassengerLastName;
            _passengerRepository.Update(passenger);

            var itinerary = new Itinerary
            {
                //UserID = userInfo.userID,
                UserID = 0,
                Stops = model.Stops,
                DepartDateTime = model.DepartureTime,
                ArrivalDateTime = model.ArrivalTime,
                AirlineReservationID = model.AirlineReservationId,
                RetStops = model.RetStops,
                TripType = (short?) int.Parse(model.TripTypeName),
                From = model.DepartureName,
                To = model.ArrivalName,
                CreationDate = DateTime.UtcNow,
                Type = "Real Flight",
                PassengerID = passenger != null ? passenger.PassengerID : 0,
                DepartDateTimeUTC = model.DepartDateTimeUTC,
                ArrivalDateTimeUTC = model.ArrivalDateTimeUTC
            };

            try
            {
                //insert the new itinerary
                _itinerarysRepository.Add(itinerary);
                var itineraryId = itinerary.ItineraryID;

                if (itineraryId > 0)
                {
                    var connxysWcfRestService = new ConnxysWCFRestService();
                    var flightsList = new StringBuilder();
                    var currentFlight = 0L;
                    var flightInd = 0;

                    //insert all the flights
                    foreach (var flightItem in _listF)
                    {
                        flightItem.ItineraryID = itineraryId;
                        _flightsRepository.Add(flightItem);

                        if (flightInd == (_listF.Count - 1))
                            flightsList.Append(flightItem.FlightID);
                        else
                        {
                            flightsList.Append(flightItem.FlightID).Append("-");
                        }

                        if (flightInd == 0)
                            currentFlight = flightItem.FlightID;

                        
                        var monitoredFlights = new MonitoredFlights();
                        {
                            monitoredFlights.FlightId = flightItem.FlightID;
                            monitoredFlights.FlightNumber = flightItem.FlightNumber;
                            monitoredFlights.StartDate = Convert.ToDateTime(flightItem.DepartureDateTime);
                            monitoredFlights.EndDate = Convert.ToDateTime(flightItem.ArrivalDateTime);
                            monitoredFlights.DepartureLocation = flightItem.DepartureLocation;
                            monitoredFlights.ArrivalLocation = flightItem.ArrivalLocation;
                            monitoredFlights.Status = 0;

                            var airline = _airlinesRepository.Get(flightItem.AirlineID);
                            monitoredFlights.AirlineDesignator = airline.AirlineDesignator;
                            monitoredFlights.AirlineName = airline.AirlineName;
                        }

                        //Insert monitoredflight in the database
                        _monitoredFlightsRepository.Add(monitoredFlights);

                        // Increase Flight index
                        flightInd++;
                    }
                    
                    //insert all the bags
                    foreach (var bagItem in _listB)
                    {                        
                        var baggageInfo = new BaggageInfo {
                            BagIATANumber = bagItem.BagIATANumber,
                            ItineraryId = itineraryId,
                            PingerId = bagItem.ConnxysTagID,
                            PassengerEmail = passenger.PassengerEmail,
                            PassengerLastName = passenger.PassengerLastName,
                            PassengerName = passenger.PassengerName
                        };

                        connxysWcfRestService.UpdateBaggageInfo(baggageInfo);

                        var itineraryPinger = new ItineraryPingers {
                            ItineraryID = itinerary.ItineraryID,
                            ConnxysTagID = bagItem.ConnxysTagID
                        };

                        _itineraryPingersRepository.Add(itineraryPinger);
                    }
                    _listB = new List<BagTagDesignation>();

                    var creationDate = DateTime.Now.ToUniversalTime();
                    _storeProcedureRepository = new StoreProcedureRepository();
                    _storeProcedureRepository.CreateOutgoingMessage(itinerary.ItineraryID, null, null, null, null, Convert.ToInt64(model.TagId), "Manually", creationDate, false);

                    //Create notification message to sent a message to the passenger abput the itinerary creation
                    var notification = new NotificationMessage
                    {
                        Title = "Itinerary Information",
                        Message = string.Format
                        ("Hello {0}!  We’re excited to have you fly with us on flight #: {1} to {2}.  We’ll notify you when your bag is checked in, when it’s been loaded aboard your flight, and where you can reclaim it in {3}.  Enjoy your flight with us!", passenger.PassengerName, _listF[0].FlightNumber.ToString(), model.ArrivalName, model.ArrivalName),
                        TokenId = passenger.TokenId
                    };

                    foreach (var fl in _listF)
                    {
                        var airlineDesignator = _airlinesRepository.Get(fl.AirlineID);

                        var callUpdateFlight = new ConnxysWCFRestService();
                        if (fl.DepartureDateTime == null) continue;
                        if (fl.ArrivalDateTime != null)
                        {
                            callUpdateFlight.UpdateSimpleFlight(
                                fl.FlightID.ToString(CultureInfo.InvariantCulture),
                                airlineDesignator.AirlineDesignator,
                                fl.FlightNumber.ToString(CultureInfo.InvariantCulture),
                                fl.DepartureDateTime.Value.ToString(CultureInfo.InvariantCulture),
                                fl.ArrivalDateTime.Value.ToString(CultureInfo.InvariantCulture));
                        }
                    }

                    _listF = new List<Flights>();
                    
                    jsonData = new {
                        status = "Ok",
                        itineraryId = itinerary.ItineraryID
                    };

                    //Send push notification about itinerary creation to the passenger
                    connxysWcfRestService.SendPushNotification(notification);
                   
                }
                else
                {
                    //throw error the itinerary can not be created
                    _listF = new List<Flights>();
                    PopulateList();
                    jsonData = new
                    {
                        status = "Failed",
                        message = "Error: The itinerary was not added."
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                //throw error the itinerary can not be created
                _listF = new List<Flights>();
                jsonData = new
                {
                    status = "Failed",
                    message = "Error: The itinerary was not added."
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }          

            #endregion

        }
        
        /// <summary>
        /// Action to Edit an Itinerary by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(long id)
        {
            _itinerarysRepository = new ItinerarysRepository();
            _flightsRepository=new FlightsRepository();
            ViewBag.flightList = _flightsRepository.GetFlightsByItinerary(id);

            return View(_itinerarysRepository.Get(id));
        }

        /// <summary>
        /// Action to Edit an Itinerary by model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Itinerary model)
        {
            if (ModelState.IsValid)
            {
                _itinerarysRepository = new ItinerarysRepository();
                var currentItinerary = _itinerarysRepository.Get(model.ItineraryID);

                model.From = currentItinerary.From;
                model.To = currentItinerary.To;
                model.RetStops = currentItinerary.RetStops;
                model.TripType = currentItinerary.TripType;
                model.PassengerID = currentItinerary.PassengerID;
                _itinerarysRepository.Update(model);
                ViewBag.SuccessMessage = "The itinerary was updated successfully.";
            }
            _flightsRepository=new FlightsRepository();
            ViewBag.flightList = _flightsRepository.GetFlightsByItinerary(model.ItineraryID);

            return View(model);
        }

        /// <summary>
        /// Action to delete an itinerary by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(long id)
        {
            _itinerarysRepository=new ItinerarysRepository();

            var res = _itinerarysRepository.Delete(id);
            if (res.Equals("NotAllowed")) 
            {
                TempData["messageDelete"] = "The operation is not allowed because there are entries in the Tag Report";
            }
            else if (res.Equals("Ok"))
            {
                TempData["messageDelete"] = "The Itinerary was successfully deleted";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Action to delete an Itinerary by model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(Itinerary model)
        {
            try
            {
                _itinerarysRepository=new ItinerarysRepository();
                _bagTagDesignationRepository = new BagTagDesignationRepository();

                _bagTagDesignationRepository.DeleteByItinerary(model.ItineraryID);
                _itinerarysRepository.Delete(model.ItineraryID);

                ViewBag.Message = "The itinerary with the Id {model.ItineraryID} was deleted from the Database";
                return View("messages");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("",e);
                ViewBag.Message =
                    string.Format(
                        "Error: an error occurred when tried to delete the itinerary with the ID {0} was not deleted from the Database",
                        model.ItineraryID);
                return View("messages");
            }
        }

        /// <summary>
        /// Get tag information
        /// </summary>
        /// <returns></returns>
        private List<string> GetConnxysTagId()
        {
            _tagsRepository = new TagsRepository();
            var tag = _tagsRepository.All();
            var result = tag.Select(item => item.ConnxysTagID).ToList();
            return result;
        }

        /// <summary>
        /// Fill a list
        /// </summary>
        private void PopulateList()
        {
            ViewBag.UserList = _listB;
            ViewBag.stopList = Web.Content.Common.Utils.StopList;
            ViewBag.hoursList = Web.Content.Common.Utils.HoursList;
            ViewBag.minutesList = Web.Content.Common.Utils.MinutesList;
            ViewBag.AirlineList = GetAirlines();
            ViewBag.AirportList = GetAirports();
            ViewBag.TypeTripList = getTypeTrip();
            ViewBag.tag = GetConnxysTagId();
        }

        /// <summary>
        /// Get type of trip
        /// </summary>
        /// <returns></returns>
        private SelectList getTypeTrip()
        {
            var tripList = new SelectList(new[] {
                new {Type = "0", Name = "One Way"},
            }, "Type", "Name", 0);

            return tripList;
        }

        /// <summary>
        /// Get airlines information
        /// </summary>
        /// <returns></returns>
        private string GetAirlines()
        {
            _airlinesRepository = new AirlinesRepository();
            var airlines = _airlinesRepository.All().ToList();
            var result = airlines.Select(item => item.AirlineName).ToList();

            var airlineNameList = "";
            if (result.Count > 0)
            {
                var totalConnxysTagIDs = result.Count;

                for (var i = 0; i < totalConnxysTagIDs; i++)
                {
                    airlineNameList += (result[i] + ",");
                }
            }

            return airlineNameList;
        }

        /// <summary>
        /// Get Airports information
        /// </summary>
        /// <returns></returns>
        private List<string> GetAirports()
        {
            _airportsRepository = new AirportsRepository();
            var airports = _airportsRepository.All();
            return airports.Where(x=>x.IsTestAirport==false).Select(item => item.AirportDesignationLetters).ToList();
        }

        /// <summary>
        /// Generate report by ItineraryId
        /// </summary>
        /// <param name="itineraryId"></param>
        /// <returns></returns>
        public ActionResult GenerateReport(long itineraryId)
        {
            _airportsRepository = new AirportsRepository();
            _itinerarysRepository = new ItinerarysRepository();
            _usersRepository = new UsersRepository();

            var itineraryInfo = _itinerarysRepository.Get(itineraryId);
            var userInfo = new Users();
            _listF = new List<Flights>();

            if (itineraryInfo != null)
            {
                userInfo = _usersRepository.Get(itineraryInfo.UserID);

                foreach (var flight in itineraryInfo.Flights)
                {
                    flight.DepartureLocation = _airportsRepository.Get(flight.DepartureLocation).AirportID;
                    flight.ArrivalLocation = _airportsRepository.Get(flight.ArrivalLocation).AirportID;
                }
            }

            ViewBag.itineraryInfo = itineraryInfo;
         
            return View();
        }

        /// <summary>
        /// View
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            return View();
        }

        /// <summary>
        /// GridData by page and rows
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ActionResult GridData(int page, int rows)
        {
            try
            {
                _itinerarysRepository = new ItinerarysRepository();
                _storeProcedureRepository = new StoreProcedureRepository();

                var res = _storeProcedureRepository.GetManageItineraries(page, rows);

                var pageSize = rows;
                var totalRecords = res.Any() ? Convert.ToInt32(res[0].intTotalHits) : 0;
                var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);             
                
                var res2 = res.ToList();
                #region

                try
                {
                    var request = ControllerContext.HttpContext.Request;
                    if (request["ItineraryID"] != null)
                    {
                        var iId = long.Parse(request["ItineraryID"]);
                        var res3 = res2.FindAll(x => x.ItineraryID == iId);
                        res2 = res3;
                    }
                   
                    else if (request["Airline"] != null)
                    {
                        var res3 =
                            res2.FindAll(
                                x => x.AirlineName.Contains(request["Airline"].ToString(CultureInfo.InvariantCulture)));
                        res2 = res3;
                    }                    

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("", exception);
                }

                #endregion

                var jsonData = new 
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = ( 
                        from x in res2
                        let departDateTime = x.DepartDateTime
                        where departDateTime != null
                        
                        let arrivalDateTime = x.ArrivalDateTime
                        where arrivalDateTime != null
                        select new 
                        {
                            id = x.ItineraryID,
                            cell = new[] { "","", "",
                                x.ItineraryID.ToString(CultureInfo.InvariantCulture),
                                departDateTime.Value.ToString(CultureInfo.InvariantCulture),
                                arrivalDateTime.Value.ToString(CultureInfo.InvariantCulture),
                                x.DepartureAirportDesignationLetters,
                                x.ArrivalAirportDesignationLetters,
                                x.FlightNumber.ToString(CultureInfo.InvariantCulture),         
                                x.AirlineName, 
                                x.PassengerName,
                                x.PassengerEmail,
                                x.Pingers,      
                                x.Stops.HasValue ? x.Stops.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                                x.Type,
                                x.IsTestItinerary == false ? "No":"Yes"
                            }
                        }).ToArray()
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                ViewBag.message = "An error was ocurred.";
                return null;
            }
        }
        
        /// <summary>
        /// Get list of tags Id's
        /// </summary>
        /// <returns></returns>
        private List<string> GetConnxysTagsIds()
        {
            _tagsRepository = new TagsRepository();

            var tags = _tagsRepository.AllAssignedTags();
            return tags.Select(item => item.ConnxysTagID != null  ? item.ConnxysTagID.ToString() : string.Empty).ToList();
        }
    }
}