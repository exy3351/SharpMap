﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using System.Xml.Serialization;
using SharpMap.Serialization;
using System.IO;
using System.Net;

namespace UnitTests.Serialization
{
    [TestFixture]
    class MapDocSerializationTests
    {
        [Test]
        public void TestSerializeWmsLayer()
        {
            SharpMap.Map m = new SharpMap.Map();
            SharpMap.Layers.WmsLayer l = new SharpMap.Layers.WmsLayer("testwms", "http://mapus.jpl.nasa.gov/wms.cgi?");
            l.AddChildLayers(l.RootLayer, false);
            m.Layers.Add(l);
            MemoryStream ms = new MemoryStream();
            SharpMap.Serialization.MapSerialization.SaveMapToStream(m, ms);
            string txt = System.Text.ASCIIEncoding.ASCII.GetString(ms.ToArray());
            Console.WriteLine(txt);
            Assert.IsTrue(txt.Contains(@"<Layers>
    <MapLayer xsi:type=""WmsLayer"">
      <Name>testwms</Name>
      <MinVisible>0</MinVisible>
      <MaxVisible>1.7976931348623157E+308</MaxVisible>
      <OnlineURL>http://mapus.jpl.nasa.gov/wms.cgi?</OnlineURL>
      <WmsLayers>global_mosaic,global_mosaic_base,us_landsat_wgs84,srtm_mag,daily_planet,daily_afternoon,BMNG,modis,huemapped_srtm,srtmplus,worldwind_dem,us_ned,us_elevation,us_colordem,gdem</WmsLayers>
    </MapLayer>
  </Layers>"));
            ms.Close();
        }

        [Test]
        public void TestDeSerializeWmsLayer()
        {

            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(@"<?xml version=""1.0""?>
<MapDefinition xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <BackGroundColor>Transparent</BackGroundColor>
  <Extent>
    <Xmin>-0.5</Xmin>
    <Ymin>-0.375</Ymin>
    <Xmax>0.5</Xmax>
    <Ymax>0.375</Ymax>
  </Extent>
  <Layers>
    <MapLayer xsi:type=""WmsLayer"">
      <Name>testwms</Name>
      <MinVisible>0</MinVisible>
      <MaxVisible>1.7976931348623157E+308</MaxVisible>
      <OnlineURL>http://mapus.jpl.nasa.gov/wms.cgi?</OnlineURL>
    </MapLayer>
  </Layers>
  <SRID>4326</SRID>
</MapDefinition>
"));
            SharpMap.Map m = SharpMap.Serialization.MapSerialization.LoadMapFromStream(ms);
            Assert.AreEqual(4326, m.SRID);
            Assert.AreEqual(1, m.Layers.Count);
            Assert.IsInstanceOfType(typeof(SharpMap.Layers.WmsLayer), m.Layers[0]);
            Assert.AreEqual("http://mapus.jpl.nasa.gov/wms.cgi?", (m.Layers[0] as SharpMap.Layers.WmsLayer).CapabilitiesUrl);
            ms.Close();
        }

        [Test]
        public void TestDeSerializeWmsLayerWithCredentials()
        {

            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(@"<?xml version=""1.0""?>
<MapDefinition xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <BackGroundColor>Transparent</BackGroundColor>
  <Extent>
    <Xmin>-0.5</Xmin>
    <Ymin>-0.375</Ymin>
    <Xmax>0.5</Xmax>
    <Ymax>0.375</Ymax>
  </Extent>
  <Layers>
    <MapLayer xsi:type=""WmsLayer"">
      <Name>testwms</Name>
      <MinVisible>0</MinVisible>
      <MaxVisible>1.7976931348623157E+308</MaxVisible>
      <OnlineURL>http://mapus.jpl.nasa.gov/wms.cgi?</OnlineURL>
      <WmsUser>test</WmsUser>
      <WmsPassword>pw</WmsPassword>
    </MapLayer>
  </Layers>
  <SRID>4326</SRID>
</MapDefinition>
"));
            SharpMap.Map m = SharpMap.Serialization.MapSerialization.LoadMapFromStream(ms);
            Assert.AreEqual(4326, m.SRID);
            Assert.AreEqual(1, m.Layers.Count);
            Assert.IsInstanceOfType(typeof(SharpMap.Layers.WmsLayer), m.Layers[0]);
            Assert.AreEqual("http://mapus.jpl.nasa.gov/wms.cgi?", (m.Layers[0] as SharpMap.Layers.WmsLayer).CapabilitiesUrl);
            Assert.IsNotNull((m.Layers[0] as SharpMap.Layers.WmsLayer).Credentials);
            Assert.IsInstanceOfType(typeof(NetworkCredential), (m.Layers[0] as SharpMap.Layers.WmsLayer).Credentials);
            Assert.AreEqual("test", ((m.Layers[0] as SharpMap.Layers.WmsLayer).Credentials as NetworkCredential).UserName);
            Assert.AreEqual("pw", ((m.Layers[0] as SharpMap.Layers.WmsLayer).Credentials as NetworkCredential).Password);
            ms.Close();
        }


        [Test]
        public void TestSerializeWmsLayerWithCredentials()
        {
            SharpMap.Map m = new SharpMap.Map();
            SharpMap.Layers.WmsLayer l = new SharpMap.Layers.WmsLayer("testwms", "http://mapus.jpl.nasa.gov/wms.cgi?", TimeSpan.MaxValue, 
                System.Net.WebRequest.DefaultWebProxy, new NetworkCredential("test", "pw"));
            m.Layers.Add(l);
            MemoryStream ms = new MemoryStream();
            SharpMap.Serialization.MapSerialization.SaveMapToStream(m, ms);
            string txt = System.Text.ASCIIEncoding.ASCII.GetString(ms.ToArray());
            Assert.IsTrue(txt.Contains(@"<Layers>
    <MapLayer xsi:type=""WmsLayer"">
      <Name>testwms</Name>
      <MinVisible>0</MinVisible>
      <MaxVisible>1.7976931348623157E+308</MaxVisible>
      <OnlineURL>http://mapus.jpl.nasa.gov/wms.cgi?</OnlineURL>
      <WmsLayers>global_mosaic,global_mosaic_base,us_landsat_wgs84,srtm_mag,daily_planet,daily_afternoon,BMNG,modis,huemapped_srtm,srtmplus,worldwind_dem,us_ned,us_elevation,us_colordem,gdem</WmsLayers>
      <WmsUser>test</WmsUser>
      <WmsPassword>pw</WmsPassword>
    </MapLayer>
  </Layers>"));
            ms.Close();
        }
    }
}