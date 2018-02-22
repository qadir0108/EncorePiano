using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using WFP.ICT.Enums;

namespace WFP.ICT.Data
{
    public class TestDataSeeder
    {
        public static void Seed(WFPICTContext context)
        {
            #region Order

            Guid typeID = Guid.NewGuid();
            // Piano Types
            foreach (var ptype in EnumHelper.GetEnumTextValues(typeof(PianoTypeEnum)))
            {

                var already = context.PianoTypes.FirstOrDefault(m => m.Code == ptype.Value);
                if (already == null)
                {
                   
                    context.PianoTypes.Add(new PianoType()
                    { 
                        Id = ptype.Text == "Spinet" ? typeID : Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        Code = ptype.Value,
                        Type = ptype.Text,
                    });
                }
            }
            context.SaveChanges();

            // Piano Finish
            foreach (var finish in EnumHelper.GetEnumTextValues(typeof(PianoFinishEnum)))
            {

                var already = context.PianoFinish.FirstOrDefault(m => m.Code == finish.Value);
                if (already == null)
                {

                    context.PianoFinish.Add(new PianoFinish()
                    {
                        Id = Guid.NewGuid(),
                        Code = finish.Value,
                        Name = finish.Text,
                        CreatedAt = DateTime.Now,
                    });
                }
            }
            context.SaveChanges();


            //PianoSizes
            context.PianoSize.Add(new PianoSize()
            {
                Id = Guid.NewGuid(),
                PianoTypeId = typeID,
                Width = 235.56,
                CreatedAt = DateTime.Now,
            });
            context.SaveChanges();

            context.PianoSize.Add(new PianoSize()
            {
                Id = Guid.NewGuid(),
                PianoTypeId = typeID,
                Width = 180.34,
                CreatedAt = DateTime.Now,
            });
            context.SaveChanges();

            // Piano Charges
            int code = 100;
            foreach (var ptype in EnumHelper.GetEnumTextValues(typeof(ChargesTypeEnum)))
            {
                string officeCode = ptype.Value;
                var already = context.PianoCharges.FirstOrDefault(m => m.ChargesType.ToString() == officeCode);
                if (already == null)
                {
                    context.PianoCharges.Add(new PianoCharges()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        ChargesCode = code,
                        ChargesType = int.Parse(ptype.Value),
                        ChargesDetails = ptype.Text,
                        Amount = 100
                    });
                    code += 100;
                }
            }
            context.SaveChanges();

            // Test Client
            var clientId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            context.Clients.Add(new Client()
            {
                Id = clientId,
                CreatedAt = DateTime.Now,
                ClientType = 1,
                AccountCode = "456343",
                Name = "Test Client 1",
                PhoneNumber = "12345678",
                EmailAddress = "client@test.com",
                Comment = "Its test Client",
            });
            context.SaveChanges();
            
            // Test Address
            context.Addresses.Add(new Address()
            {
                Id = addressId,
                CreatedAt = DateTime.Now,
                ClientId = clientId,
                Name = "Client Person Name",
                Address1 = "Complete Address",
                PhoneNumber = "1234",
                PostCode = "123",
                State = "CA",
                City = "City name",
                Lat = "34.504772",
                Lng="-117.221566",
            });
            context.SaveChanges();

            var client = context.Clients.FirstOrDefault();
            if(client != null)
            {
                client.AddressId = addressId;
                context.SaveChanges();
            }

            #endregion

            #region Delivery

            // Vehicles Types
            var vehicleTypeId1 = Guid.NewGuid();
            var vehicleTypeId2 = Guid.NewGuid();
            context.VehicleTypes.Add(new VehicleType()
            {
                Id = vehicleTypeId1,
                CreatedAt = DateTime.Now,
                Code = "101",
                Name = "Truck",
            });
            context.VehicleTypes.Add(new VehicleType()
            {
                Id = vehicleTypeId2,
                CreatedAt = DateTime.Now,
                Code = "102",
                Name = "Trailer",
            });
            context.SaveChanges();

            var vehicle1 = Guid.NewGuid();
            var vehicle2 = Guid.NewGuid();
            var vehicle3 = Guid.NewGuid();
            context.Vehicles.Add(new Vehicle()
            {
                Id = vehicle1,
                CreatedAt = DateTime.Now,
                Code = "5876",
                Name = "Truck ABC-5876",
                VehicleTypeId = vehicleTypeId1
            });
            context.Vehicles.Add(new Vehicle()
            {
                Id = vehicle2,
                CreatedAt = DateTime.Now,
                Code = "4272",
                Name = "Truck XYZ-4272",
                VehicleTypeId = vehicleTypeId1
            });
            context.Vehicles.Add(new Vehicle()
            {
                Id = vehicle3,
                CreatedAt = DateTime.Now,
                Code = "633",
                Name = "Trailer X-633",
                VehicleTypeId = vehicleTypeId2
            });
            context.SaveChanges();

            // Drivers
            context.Drivers.Add(new Driver()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Code = "D101",
                Password = "P@kistan1",
                Name = "Driver 1 Name",
                Description = "This is test Driver 1",
            });

            context.Drivers.Add(new Driver()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Code = "D102",
                Password = "P@kistan1",
                Name = "Driver 2 Name",
                Description = "This is Driver 2",
            });
            context.SaveChanges();

            // Warehouse
            var warehouse = new Warehouse()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Code = "W101",
                Name = "Warehouse 101",
            };
            context.Warehouses.Add(warehouse);

            var addressIdWarehouse = Guid.NewGuid();
            context.Addresses.Add(new Address()
            {
                Id = addressIdWarehouse,
                CreatedAt = DateTime.Now,
                AddressType = (int)AddressTypeEnum.Warehouse,
                WarehouseId = warehouse.Id,
                Name = "ENCORE PIANO MOVING Warehouse",
                Address1 = "15915 CANARY AVE.",
                PhoneNumber = "(714) 739-4717",
                PostCode = "90638",
                State = "CA",
                City = "LA MIRADA",
                Lat = "33.892162",
                Lng = "-118.024756"
            });
            warehouse.AddressId = addressIdWarehouse;
            context.SaveChanges();

            //Piano Make
            foreach (var ptype in EnumHelper.GetEnumTextValues(typeof(PianoMakeEnum)))
            {
                var already = context.PianoMake.FirstOrDefault(m => m.Name == ptype.Text);
                if (already == null)
                {
                    context.PianoMake.Add(new PianoMake()
                    {
                        Id = Guid.NewGuid(),
                        Code = ptype.Value,
                        Name = ptype.Text,
                        CreatedAt = DateTime.Now,
                    });
                }
            }
            context.SaveChanges();

            var locations = new string[] { "Los Angeles", "San Francisco", "Anaheim" };
            foreach (var l in locations)
            {
                context.Locations.Add(new Location()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Name = l
                });
            }
            context.SaveChanges();

            #endregion

        }
    }
}
