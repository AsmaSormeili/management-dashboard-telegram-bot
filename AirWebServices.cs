using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using us.kasra;
using System.Net.Http;

namespace AirWebServices
{
    public struct AirPort
    {
        public String IATACode;
        public String AirportNAME_EN;
        public String AirPortNAME_FA;
    }

    public struct Flight
    {
        public String FlightNo;
        public String DepartureDateTime;
        public String ArrivalDateTime;
        public String AdultTotalPrices;
        public String FlightCalssesStatus;
        public String Origin;
        public String Destination;
    }

    public struct FlightFare
    {
        public String FlightNo;
        public String DepartureDateTime;
        public String ArrivalDateTime;
        public String FlightCalssesCode;
        public String Origin;
        public String Destination;
        public int AdultTotalPrice;
        public int AdultFare;
        public int ChildTotalPrice;
        public int ChildFare;
        public int InfantTotalPrice;
        public int InfantFare;
    }

    public struct Fare {
        public int AdultFare;
        public int AdultTotalPrice;
        public int ChildFare;
        public int ChildTotalPrice;
        public int InfantFare;
        public int InfantTotalPrice;
    }
    public struct Passenger
    {
        public String FirstName;
        public String LastName;
        public String Gender;
        public KDateTime BirthDate;
        public String DocNo;
    }

    public struct Segment
    {
        public String Origin;
        public String Destination;
        public String FlightClassCode;
        public String FlightNo;
        public KDateTime FlightDate;
    }
     
    public struct TicketCopoun
    {
        public String Origin;
        public String Destination;
        public String DepartureDT;
        public String Status;
        public int Fare;
        public String PNR;
        public String PassengerFullName;
        public String FlihgtNo;
    }

    public struct Tax
    {
        public String TaxCode;
        public int TaxAmount;
    }

    public struct TicketInformation
    {
        public String PassengerFullName;
        public String TicketNo;

        public int Fare;
        public int TotalPrice;
        public int Commission;
        public String PAX;

        public Tax[] Taxes;
        public TicketCopoun[] Copouns;
    }

    public struct PassengerTicket
    {
        public String PassengerFullName;
        public String TicketCode;
    }

    public struct PassengerSegment
    {
        public String PassengerName;
        public String PassengerlastName;
        public String Origin;
        public String Destination;
        public String FlightClassCode;
        public String FlightNo;
        public KDateTime FlightDate;
        public String PassenegerTicketCode;
        public String PassengerDoc;
    }

    public struct RouteSalesData
    {
        public String Origin;
        public String Destination;
        public String FlightNo;
        public String DepartureDT;
        public int Passengers;
        public int Income;
    }

    public class AirWS
    {
        private String BaseURL=null;
        private String BaseAvailabilityURL = "";//tbSystemProperty.findUnique("BaseAvailabilityURL").Value;
        private String BaseFareURL = "";//tbSystemProperty.findUnique("BaseFareURL").Value;
     //   private String BaseOrgDestUrl = "http://zv.nirasoftware.com:880";//"http://book.zagrosairlines.com/cgi-bin/NRSWeb.cgi"; //tbSystemProperty.findUnique("BaseOrgDestUrl").Value;
        private String BaseOrgDestUrl = tbSystemProperty.findUnique("BaseOrgDestBotUrl").Value;
        private String BaseReserveUrl = "";//tbSystemProperty.findUnique("BaseReserveUrl").Value;
        private String BaseIssueETUrl = "";//tbSystemProperty.findUnique("BaseIssueETUrl").Value;
        private String BaseRetrieveUrl = "";//tbSystemProperty.findUnique("BaseRetrieveUrl").Value;
        private String OfficeUSer = "";//tbSystemProperty.findUnique("OfficeUSer").Value;
        private String OfficePass = "";//tbSystemProperty.findUnique("OfficePass").Value;
        private String AirLine    = "";//tbSystemProperty.findUnique("AirLine").Value;

        public  String  sendHttpRequest(String url)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                // by calling .Result you are performing a synchronous call
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;

                return responseString;
            } 
            return "";
        }

        public AirPort[] GetOrigins()
        {
            String url;
            if (BaseURL == null) url = BaseOrgDestUrl;
            else url = BaseURL;
            url += BaseURL + "/RoutesJS";
            String jsonResponse = sendHttpRequest(url);
            JObject CitiesJO = JObject.Parse(jsonResponse);
            //String citiesStr = (String)CitiesJO["CITIES"];
            //JArray objects = JArray.Parse(citiesStr); // parse as array  
            JArray objects = (JArray)CitiesJO["CITIES"];
            AirPort[] ret = new AirPort[objects.Count];
            int idx = 0;
            foreach (JObject jo in objects)
            {
                try
                {
                    ret[idx].IATACode = (String)jo["CITY"];
                    ret[idx].AirportNAME_EN = (String)jo["CITYNAME_EN"];
                    ret[idx].AirPortNAME_FA = IATACodeToNameMapper((String)jo["CITY"]); //(String)jo["CITYNAME_FA"];
                    idx++;
                }
                catch (Exception e)
                {
                    return ret;
                }
            }
            return ret;
        }

        public RouteSalesData[] GetRouteSalesReport(String Origin,String Destination,String DepartureDate)
        {
            RouteSalesData[] ret = new RouteSalesData[2];

            ret[0].Origin="THR";
            ret[0].Destination="AWZ";
            ret[0].FlightNo = "4017";
            ret[0].DepartureDT = "2018-01-21 17:00";
            ret[0].Passengers = 163;
            ret[0].Income = 1445000000;

            ret[1].Origin = "THR";
            ret[1].Destination = "AWZ";
            ret[1].FlightNo = "4018";
            ret[1].DepartureDT = "2018-01-21 22:00";
            ret[1].Passengers = 153;
            ret[1].Income = 1845000000;

            return ret;
        }

        public AirPort[] GetDestinations(String OriginIATACode)
        {
            String url;
            if (BaseURL == null) url = BaseOrgDestUrl;
            else  url = BaseURL;
            url +=  "/RoutesJS?origin=" + OriginIATACode;
            String jsonResponse = sendHttpRequest(url);
            JObject CitiesJO = JObject.Parse(jsonResponse);
            //String citiesStr = (String)CitiesJO["CITIES"];
            //JArray objects = JArray.Parse(citiesStr); // parse as array  
            JArray objects = (JArray)CitiesJO["CITIES"];
            AirPort[] ret = new AirPort[objects.Count];
            int idx = 0;
            foreach (JObject jo in objects)
            {
                try
                {
                    ret[idx].IATACode = (String)jo["CITY"];
                    ret[idx].AirportNAME_EN = (String)jo["CITYNAME_EN"];
                    ret[idx].AirPortNAME_FA = IATACodeToNameMapper((String)jo["CITY"]);//(String)jo["CITYNAME_FA"];
                    idx++;
                }
                catch (Exception e)
                {
                    return ret;
                }
            }
            return ret;
        }

        public Flight[] GetAvailability(String Origin,String Destination,String DepartureDate)
        {
            String url;
            if (BaseURL == null) url = BaseAvailabilityURL;
            else url = BaseURL;
            KDateTime kdt = new KDateTime();
            kdt = kdt.setDate(DepartureDate);
            url += BaseURL + "/AvailabilityJS.jsp?AirLine=" + AirLine + "&cbSource=" + Origin + "&cbTarget=" + Destination + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass + "&cbDay1=" + kdt.getShamsiDayOfMonth() + "&cbMonth1=" + kdt.getShamsiMonthOfYear();
            String jsonResponse = sendHttpRequest(url);
            JObject CitiesJO = JObject.Parse(jsonResponse);
            //String citiesStr = (String)CitiesJO["CITIES"];
            //JArray objects = JArray.Parse(citiesStr); // parse as array  
            JArray objects = (JArray)CitiesJO["AvailableFlights"];
            Flight[] ret = new Flight[objects.Count];
            int idx = 0;
            foreach (JObject jo in objects)
            {
                try
                {
                    ret[idx].DepartureDateTime = (String)jo["DepartureDateTime"];
                    ret[idx].ArrivalDateTime = (String)jo["ArrivalDateTime"];
                    ret[idx].AdultTotalPrices = (String)jo["AdultTotalPrices"];
                    ret[idx].FlightCalssesStatus = (String)jo["ClassesStatus"];
                    ret[idx].FlightNo = (String)jo["FlightNo"];
                    ret[idx].Origin = (String)jo["Origin"];
                    ret[idx].Destination = (String)jo["Destination"];
                    
                    char[] delimiterChars = {' ' };
                    String[] Classes = ret[idx].FlightCalssesStatus.Split(delimiterChars);
                    String[] AdultPrices = ret[idx].AdultTotalPrices.Split(delimiterChars);
                    idx++;
                }
                catch (Exception e)
                {
                    return ret;
                }
            }
            return ret;
        }

        public FlightFare[] GetAvailableFlightFares(String Origin, String Destination, String DepartureDate)
        {
            Flight[] flights = GetAvailability(Origin, Destination, DepartureDate);
            int n = flights.GetLength(0);
            int nret = 0;
            for (int i = 0; i < n; i++)
            {
                char[] delimiterChars = { ' ' };
                String[] Classes = flights[i].FlightCalssesStatus.Split(delimiterChars);
                int nClasses = Classes.GetLength(0);
                String[] AdultPrices = flights[i].AdultTotalPrices.Split(delimiterChars);
                int nPrices = AdultPrices.GetLength(0);
                for (int j = 0; j < nClasses; j++)
                {
                    if ( (Classes[j][1]=='A') || ( (Classes[j][1]>='1') && (Classes[j][1]<='9') ) ) 
                    {
                        char flightClassCode = Classes[j][0];
                        for (int k = 0; k < nPrices; k++)
                        {
                            if ((AdultPrices[k][0] == Classes[j][0]) && (AdultPrices[k][2] != '-'))
                                nret++;
                        }
                    }

                }
            }


            FlightFare[] ret = new FlightFare[nret];
            nret=0;
            for (int i = 0; i < n; i++)
            {
                char[] delimiterChars = { ' ' };
                String[] Classes = flights[i].FlightCalssesStatus.Split(delimiterChars);
                int nClasses = Classes.GetLength(0);
                String[] AdultPrices = flights[i].AdultTotalPrices.Split(delimiterChars);
                int nPrices = AdultPrices.GetLength(0);
                for (int j = 0; j < nClasses; j++)
                {
                    if ((Classes[j][1] == 'A') || ((Classes[j][1] >= '1') && (Classes[j][1] <= '9')))
                    {
                        char flightClassCode = Classes[j][0];
                        for (int k = 0; k < nPrices; k++)
                        {
                            if ((AdultPrices[k][0] == Classes[j][0]) && (AdultPrices[k][2] != '-'))
                            {
                                ret[nret].FlightNo = flights[i].FlightNo;
                                ret[nret].DepartureDateTime = flights[i].DepartureDateTime;
                                ret[nret].ArrivalDateTime = flights[i].ArrivalDateTime ;
                                ret[nret].Origin = flights[i].Origin;
                                ret[nret].Destination = flights[i].Destination;
                                ret[nret].AdultTotalPrice = Convert.ToInt32(AdultPrices[k].Substring(2));
                                ret[nret].FlightCalssesCode = (String)Classes[j].Substring(0, 1);
                                nret++;
                            }
                        }
                    }

                }
            }

            return ret;
        }


        public Fare GetRouteCompleteFare(String Origin, String Destination,String DepRBD,String RetRBD ,bool RoundTrip)
        {
            String url;
            if (BaseURL == null) url = BaseFareURL;
            else url = BaseURL;
            String Route = Origin + "-" + Destination;
            if (RoundTrip)
                Route += "-" + Origin;
            url += BaseURL + "/FareJS.jsp?AirLine=" + AirLine + "&Route=" + Route + "&RBD=" + DepRBD + "&RBDRet=" + RetRBD + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass;
            String jsonResponse = sendHttpRequest(url);
            JObject jo = JObject.Parse(jsonResponse);
            Fare ret = new Fare();
            ret.AdultFare = Convert.ToInt32((String)jo["AdultFare"]);
            ret.AdultTotalPrice = Convert.ToInt32((String)jo["AdultTotalPrice"]);
            ret.ChildFare = Convert.ToInt32((String)jo["ChildFare"]);
            ret.ChildTotalPrice = Convert.ToInt32((String)jo["ChildTotalPrice"]);
            ret.InfantFare = Convert.ToInt32((String)jo["InfantFare"]);
            ret.InfantTotalPrice = Convert.ToInt32((String)jo["InfantTotalPrice"]);
            return ret;
        }

        public String MakeReservation(Passenger[] Passengers,Segment[] Segments,String Contact)
        {
            String PNR = "";
            String url;
            if (BaseURL == null) url = BaseReserveUrl;
            else url = BaseURL;

            
                url += BaseURL + "/ReservJS?AirLine=" + AirLine + "&cbSource=" + Segments[0].Origin + "&cbTarget=" + Segments[0].Destination
                + "&FlightClass=" + Segments[0].FlightClassCode + "&FlightNo=" + Segments[0].FlightNo
                + "&Day=" + Convert.ToString(Segments[0].FlightDate.getShamsiDayOfMonth()) + "&Month=" + Convert.ToString(Segments[0].FlightDate.getShamsiMonthOfYear())
                + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass
                + "&edtContact=" + Contact + "&No=" + Convert.ToString(Passengers.Length);
            if (Segments.Length > 1)
                url += "&TwoLeg=YES&FlightClassRet=" + Segments[1].FlightClassCode + "&FlightNoRet=" + Segments[1].FlightNo
                    + "&DayRet=" + Convert.ToString(Segments[1].FlightDate.getShamsiDayOfMonth())
                    + "&MonthRet=" + Convert.ToString(Segments[1].FlightDate.getShamsiMonthOfYear());
            int n = Passengers.Length;
            KDateTime Now = new KDateTime();
            for (int i = 0; i < n; i++)
            {
                int days = Now.daysBetween(Passengers[i].BirthDate);
                int age = days / 365;
                url += "&edtName" + Convert.ToString(i + 1) + "=" + Passengers[i].FirstName
                    + "&edtLast" + Convert.ToString(i + 1) + "=" + Passengers[i].LastName
                    + "&edtAge" + Convert.ToString(i + 1) + "=" + Convert.ToString(age)
                    + "&EdtID" + Convert.ToString(i + 1) + "=" + Passengers[i].DocNo;
            }

            String jsonResponse = sendHttpRequest(url);
            JObject ResJO = JObject.Parse(jsonResponse);
            JArray objects = (JArray)ResJO["AirReserve"];
            PNR = (String)objects[0]["PNR"];

            return PNR;
        }

        public PassengerTicket[] IssueTicket(String PNR, String Email)
        {
            String url;
            if (BaseURL == null) url = BaseIssueETUrl;
            else url = BaseURL;
            PassengerTicket[] ret = null;
            url += String.Format("{0}/ETIssueJS?AirLine={1}&PNR={2}&EMail={3}&OfficeUser={4}&OfficePass={5}", BaseURL, AirLine, PNR, Email, OfficeUSer, OfficePass);
            String jsonResponse = sendHttpRequest(url);
            JObject ResJO = JObject.Parse(jsonResponse);
            JArray objects = (JArray)ResJO["AirNRSTICKETS"];
            String Tickets = (String)objects[0]["Tickets"];
            String[] PassengerTickets = Tickets.Split(new String[] {"\r","\n"}, StringSplitOptions.RemoveEmptyEntries);
            ret = new PassengerTicket[PassengerTickets.Length];
            for (int i=0;i<PassengerTickets.Length;i++)
            {
                String[] PT = PassengerTickets[i].Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                ret[i].PassengerFullName = PT[0];
                ret[i].TicketCode = PT[1];
            }
            return ret;

        }

        public PassengerSegment[] RetrivePNR(String PNR)
        {
            PassengerSegment[] ret = null;
            String url;
            if (BaseURL == null) url = BaseRetrieveUrl;
            else url = BaseURL;
            url += BaseURL + "/NRSRT.jsp?AirLine=" + AirLine + "&PNR=" + PNR + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass;
            String jsonResponse = sendHttpRequest(url);
            JObject RTJO = JObject.Parse(jsonResponse);
            JArray  Passengers = (JArray)RTJO["Passengers"];
            JArray  Segments   = (JArray)RTJO["Segments"];
            JArray  Tickets    = (JArray)RTJO["Tickets"];
            int PassengerNo = Passengers.Count;
            int SegmentNo = Segments.Count;
            int n = PassengerNo * SegmentNo;
            int idx=0;
            ret = new PassengerSegment[n];
            foreach (JObject Passenger in Passengers)
                foreach (JObject Segment in Segments)
                {
                    if (idx<n)
                    {
                        ret[idx].Origin = (String)Segment["Origin"];
                        ret[idx].Destination = (String)Segment["Destination"];
                        ret[idx].FlightNo = (String)Segment["FlightNo"];
                        ret[idx].FlightClassCode = (String)Segment["FlightClass"];
                        String DT = (String)Segment["DepartureDT"];
                        ret[idx].FlightDate = new KDateTime().setMiladi(DT);
                        ret[idx].PassengerName = (String)Passenger["PassenferFirstName"];
                        ret[idx].PassengerlastName = (String)Passenger["PassenferLastName"];
                        foreach (JObject Ticket in Tickets)
                        {
                            String PassengerET = (String)Ticket["PassengerET"];
                            String PassengerName=ret[idx].PassengerlastName+"/"+ret[idx].PassengerName;
                            if (PassengerET.StartsWith(PassengerName))
                            {
                                char[] delimiterChars = { ':' };
                                String[] s = PassengerET.Split(delimiterChars);
                                if ( (s.Length>1) )
                                  ret[idx].PassenegerTicketCode = s[1];
                            }
        
                        }
                        idx++;
                        
                    }
                }
            return ret;
        }

        public TicketInformation RetriveTicket(String TicketNo)
        {
            TicketInformation ret = new TicketInformation();
            String url;
            if (BaseURL == null) url = BaseRetrieveUrl;
            else url = BaseURL;
            url += BaseURL + "/NRSETR.jsp?AirLine=" + AirLine + "&TicketNo=" + TicketNo + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass;
            String jsonResponse = sendHttpRequest(url);
            JObject ETRJO = JObject.Parse(jsonResponse);

            ret.PassengerFullName = (String)ETRJO["PassengerFullName"];
            ret.TicketNo = (String)ETRJO["TicketNo"];
            ret.TotalPrice = Convert.ToInt32((String)ETRJO["TotalPrice"]);
            ret.Fare = Convert.ToInt32((String)ETRJO["Fare"]);
            ret.PAX = ((String)ETRJO["PAX"]).Trim();

            JArray Coupouns = (JArray)ETRJO["COUPONS"];
            ret.Copouns = new TicketCopoun[Coupouns.Count];
            int idx=0;
            foreach (JObject Coupoun in Coupouns)
            {
                ret.Copouns[idx].Origin = (String)Coupoun["Origin"];
                ret.Copouns[idx].Destination = (String)Coupoun["Destination"];
                ret.Copouns[idx].Status = (String)Coupoun["Status"];
                ret.Copouns[idx].DepartureDT = (String)Coupoun["Departure"];
                ret.Copouns[idx].Fare = Convert.ToInt32((String)Coupoun["Fare"]);
                ret.Copouns[idx].PNR = (String)Coupoun["PNR"];
                ret.Copouns[idx].PassengerFullName = (String)Coupoun["PassengerFullName"];
                ret.Copouns[idx].FlihgtNo = (String)Coupoun["FlightNo"];
                idx++;
            }
            idx = 0;
            JArray Taxes = (JArray)ETRJO["TAXES"];
            ret.Taxes = new Tax[Taxes.Count];
            foreach (JObject Tax in Taxes)
            {
                ret.Taxes[idx].TaxCode = (String)Tax["TaxCode"];
                ret.Taxes[idx].TaxAmount = Convert.ToInt32((String)Tax["TaxAmount"]);
                idx++;
            }

            return ret;
        }

        public int GetRefundPenalty(String TicketCode)
        {
            int  ret = -1;
            String url;
            if (BaseURL == null) url = BaseRetrieveUrl;
            else url = BaseURL;
            url += BaseURL + "/NRSPenaltyNow.jsp?AirLine=" + AirLine + "&TicketNo=" + TicketCode + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass;
            String jsonResponse = sendHttpRequest(url);
            JObject RespO = JObject.Parse(jsonResponse);
            try
            {
                JObject PenaltyO = (JObject)RespO["NRSPenalty"];
                String PenaltyStr = (String)PenaltyO["PENALTY"];
                if (PenaltyStr != null)
                    ret = Convert.ToInt32(PenaltyStr);
            }
            catch (Exception e)
            {

            }
            return ret;
        }

        public int CancelSeat(String PNR,String PassengerName,String PassengerLastName,String DepartureDate,String FlightNo)
        {
            int ret = 0;
            String url;
            if (BaseURL == null) url = BaseIssueETUrl;
            else url = BaseURL;
            url += BaseURL + "/CancelSeatJS?AirLine=" + AirLine + "&PNR=" + PNR + "&PassengerName=" + PassengerName + "&PassengerLastName=" + PassengerLastName + "&DepartureDate=" + DepartureDate + "&FlightNo=" + FlightNo + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass;
            String jsonResponse = sendHttpRequest(url);
            JObject ResJO = JObject.Parse(jsonResponse);
            JArray OA = (JArray)ResJO["AirCancelSeat"];
            JObject O = (JObject)OA[0];
            String Done = (String)O["Done"];
            if (Done.ToUpper().Trim().Equals("TRUE"))
                ret = 1;
            else
                ret = 0;
            return ret;
        }

        public int RefundTicket(String TicketNo, int Penalty,int RefundFare,int RefundKU,int RefundLP)
        {
            int ret = 0;
            String url;
            if (BaseURL == null) url = BaseIssueETUrl;
            else url = BaseURL;
            url += BaseURL + "/ETRefundJS?AirLine=" + AirLine + "&TicketNo=" + TicketNo + "&Penalty=" + Convert.ToString(Penalty) + "&Fare=" + Convert.ToString(RefundFare) + "&KU=" + Convert.ToString(RefundKU) + "&LP=" + Convert.ToString(RefundLP)  + "&OfficeUser=" + OfficeUSer + "&OfficePass=" + OfficePass;
            String jsonResponse = sendHttpRequest(url);
            JObject ResJO = JObject.Parse(jsonResponse);
            JArray OA = (JArray)ResJO["AirNRSRefund"];
            //JObject O = (JObject)ResJO["AirNRSRefund"];
            JObject O = (JObject)OA[0];
            String Done = (String)O["Done"];
            if (Done.ToUpper().Trim().Equals("0"))
                ret = 1;
            else
                ret = 0;
            return ret;
        }

        public String SendCommand(String cmd,String id)
        {
            String ret = "ERROR";
            String url;
            if (BaseURL == null) url = BaseOrgDestUrl;
            else url = BaseURL;
            url += "/CommandJS.jsp?Airline=" + AirLine + "&cmd=" + cmd + "&chatID=" + id;
            String jsonResponse = sendHttpRequest(url);
            //JObject ResJO = JObject.Parse(jsonResponse);
            //JObject O = (JObject)ResJO["AirNRSCommand"];
            //String R = (String)O["Response"];
            return jsonResponse;
        }

        public AirWS(String Base)
        {
            BaseURL = Base;
        }
        public AirWS()
        {
            BaseURL = null;
            OfficeUSer = "NSTHR007.jafarimaram";// tbSystemProperty.findUnique("OfficeUSer").Value;
            OfficePass = "A1111";// tbSystemProperty.findUnique("OfficePass").Value;
            AirLine    = "ZV";//.findUnique("AirLine").Value;
        }
        public AirWS(String AirLn, String Base, String User, String Pass)
        {
            BaseURL = Base;
            OfficeUSer = User;
            OfficePass = Pass;
            AirLine = AirLn;
        }
        public AirWS(String AirLn,String User, String Pass)
        {
            OfficeUSer = User;
            OfficePass = Pass;
            AirLine = AirLn;
            BaseURL = null;
        }

        public static  String IATACodeToNameMapper(String iataCode)
       {
           try
           {
               if (iataCode.Equals("THR"))
                   return "تهران";
               else if (iataCode.Equals("MHD"))
                   return "مشهد";
               else if (iataCode.Equals("IFN"))
                   return "اصفهان";
               else if (iataCode.Equals("SYZ"))
                   return "شیراز";
               else if (iataCode.Equals("BND"))
                   return "بندعباس";
               else if (iataCode.Equals("KIH"))
                   return "کیش";
               else if (iataCode.Equals("ABD"))
                   return "آبادان";
               else if (iataCode.Equals("ADB"))
                   return "ازمیر";
               else if (iataCode.Equals("PGU"))
                   return "عسلویه";
               else if (iataCode.Equals("GYD"))
                   return "باکو";
               else if (iataCode.Equals("MRX"))
                   return "فرودگاه امام";
               else if (iataCode.Equals("NJF"))
                   return "نجف";
               else if (iataCode.Equals("TBS"))
                   return "تفلیس";
               else if (iataCode.Equals("FRU"))
                   return "بیشکک";
               else if (iataCode.Equals("EVN"))
                   return "ایروان";
               else if (iataCode.Equals("ISE"))
                   return "اسپارتا";
               else if (iataCode.Equals("BGW"))
                   return "بغداد";
               else if (iataCode.Equals("OMH"))
                   return "ارومیه";
               else if (iataCode.Equals("AWZ"))
                   return "اهواز";
               else if (iataCode.Equals("AZD"))
                   return "یزد";
               else if (iataCode.Equals("BUZ"))
                   return "بوشهر";
               else if (iataCode.Equals("CQD"))
                   return "شهرکرد";
               else if (iataCode.Equals("IKA"))
                   return "فرودگاه امام";
               else if (iataCode.Equals("ADU"))
                   return "اردبیل";
               else if (iataCode.Equals("SRY"))
                   return "ساری";
               else if (iataCode.Equals("DEF"))
                   return "دزفول";
               else if (iataCode.Equals("KER"))
                   return "کرمان";
               else if (iataCode.Equals("TBZ"))
                   return "تبریز";
               else if (iataCode.Equals("KSH"))
                   return "کاشان";
               else if (iataCode.Equals("GSM"))
                   return "قشم";
               else if (iataCode.Equals("ZBR"))
                   return "چابهار";
               else if (iataCode.Equals("TTQ"))
                   return "کاستاریکا";
               return iataCode;
           }
            catch (Exception e)
           {
               return "";
           }
      }

    }
}
