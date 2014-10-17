using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Globalization;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
	public class GoogleMap
	{
		private IItem product;
		public IItem Product
		{
			get
			{
				return product;
			}
			set
			{
				product = value;
                decimal? longitude = product.GetRawValue(AttributeKeyEnum.Location, AttributeKeyEnum.Longitude);
				if (longitude != null) ImageMarkerLng = (double)longitude.Value;
                decimal? latitude = product.GetRawValue(AttributeKeyEnum.Location, AttributeKeyEnum.Latitude);
				if (latitude != null) ImageMarkerLat = (double)latitude.Value;
			}
		}

		public bool HasCoordinates
		{
			get
			{
                return product.GetRawValue(AttributeKeyEnum.Location, AttributeKeyEnum.Longitude) != null;
			}
		}

		private bool sensor = false;
		public bool Sensor
		{
			get { return sensor; }
			set { sensor = value; }
		}

		private int imageWidth = 243;
		public int ImageWidth
		{
			get { return imageWidth; }
			set { imageWidth = value; }
		}

		private int imageHeight = 175;
		public int ImageHeight
		{
			get { return imageHeight; }
			set { imageHeight = value; }
		}

		private int imageZoom = 4;
		public int ImageZoom
		{
			get { return imageZoom; }
			set { imageZoom = value; }
		}

		private double imageMarkerLat = 45.8;
		public double ImageMarkerLat
		{
			get { return imageMarkerLat; }
			set { imageMarkerLat = value; }
		}

		private double imageMarkerLng = 16.0;
		public double ImageMarkerLng
		{
			get { return imageMarkerLng; }
			set { imageMarkerLng = value; }
		}

		private string imageTitle = "mapa";
		public string ImageTitle
		{
			get { return imageTitle; }
			set { imageTitle = value; }
		}

		private double mapSizeFactor = 1;
		public double MapSizeFactor
		{
			get { return mapSizeFactor; }
			set { mapSizeFactor = value; }
		}

		private double? mapCenterLat = null;
		public double? MapCenterLat
		{
			get { return mapCenterLat; }
			set { mapCenterLat = value; }
		}

		private double? mapCenterLng = null;
		public double? MapCenterLng
		{
			get { return mapCenterLng; }
			set { mapCenterLng = value; }
		}

		private int? mapZoom = null;
		public int? MapZoom
		{
			get { return mapZoom; }
			set { mapZoom = value; }
		}

		private int? mapMaxWidth = null;
		public int? MapMaxWidth
		{
			get { return mapMaxWidth; }
			set { mapMaxWidth = value; }
		}

		private int? mapMaxHeight = null;
		public int? MapMaxHeight
		{
			get { return mapMaxHeight; }
			set { mapMaxHeight = value; }
		}

		private string locationId = null;
		public string LocationId
		{
			get { return locationId; }
			set
			{
				locationId = value;
			}
		}

		public GoogleMap()
		{
			ImageMarkerLat = 45.789552;
			ImageMarkerLng = 15.95511;
			ImageZoom = 14;
			MapZoom = 16;
			MapSizeFactor = 0.8f;
		}
	}
}
