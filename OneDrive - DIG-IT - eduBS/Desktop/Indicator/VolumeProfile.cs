#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class MyCustomIndicator : Indicator
	{
		// Class Level Arrays to not load it each time another bar appears
		private double[] volumeProfile;
		int pocRow;
		double pocPrice;
		double maxVolume;
		double totalVolume;
		double targetVolume;
		double accumulatedVolume;
		int upperRow;
		int lowerRow;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"My First Indicator, im making a basic Volume Profile as a project to get better in C#";
				Name										= "MyFirstIndicatorVP";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Lookback					= 100;
				Rows					= 100;
				ValueArea					= 70;
			}
			
			// Loading the Data before the Calculation
			else if (State == State.DataLoaded){
				volumeProfile = new double[Rows];
				
			}
			
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <Lookback - 1){
			return;
			}
			
			for  (int i = 0; i < Rows; i++){
				volumeProfile[i] = 0;
			}
			
			double highest = High[0];
			double lowest = Low[0];
			
			for (int i = 0; i < Lookback; i++)
			{
				if(highest < High[i]){
					highest = High[i];
				}
				if (lowest > Low[i]){
					lowest = Low[i];
				}
				
				
			}
			
			double priceRange = highest - lowest;
			double rowSize =  priceRange / Rows;
			
			Print ("Range: " + priceRange);
			Print("Row Size: " + rowSize);
			Print("Highest: "+ highest);
			Print("Lowest: "+ lowest);
			
			for(int i = 0; i < Lookback; i++){
				
				double calculate = (Close[i] - lowest) / rowSize;
				int row = (int)calculate;
				
				if (row < 0){
        		row = 0;
    			}
			    else if (row >= volumeProfile.Length)
			    {
			        row = volumeProfile.Length - 1;
			    }
				
				volumeProfile[row] += Volume[i];
				
			}
			for(int i = 0; i < Rows; i++){
				if(volumeProfile[i] > 0){
					Print("Row: " + i + " | Volume: " + volumeProfile[i]);
				}
			}
			
			for(int i = 0; i < Rows; i++){
				if(volumeProfile[i] > maxVolume){
					
					pocRow = i;
					maxVolume = volumeProfile[i];
				}
			}
			pocPrice = lowest + (pocRow * rowSize);
			upperRow = pocRow;
			lowerRow = pocRow;
			
			
			Print("POC Row: " +pocRow + " | POC Price: " + pocPrice + " | Most Volume: " + maxVolume);
			
			for (int i = 0; i < Rows; i++){
				totalVolume += volumeProfile[i];
			}
			
			accumulatedVolume = maxVolume;
			targetVolume = totalVolume / 100 * ValueArea;
			
			while(accumulatedVolume < targetVolume){
				
			int above = upperRow + 1;
			int below = lowerRow -1;
			
			if(volumeProfile[above] > volumeProfile[below] ){
				upperRow++;
				accumulatedVolume += volumeProfile[upperRow];
			}
			else{
				lowerRow--;
				accumulatedVolume += volumeProfile[lowerRow];
			}
			}
		}
		
		

		#region Properties
		[NinjaScriptProperty]
		[Range(10, int.MaxValue)]
		[Display(Name="Lookback", Description="Numbers of bars used for the Volume profile", Order=1, GroupName="Parameters")]
		public int Lookback
		{ get; set; }

		[NinjaScriptProperty]
		[Range(100, int.MaxValue)]
		[Display(Name="Rows", Description="How many rows the Volume Profile has ", Order=2, GroupName="Parameters")]
		public int Rows
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, 100)]
		[Display(Name="ValueArea", Description="Percentage of Volume used for the Value Area", Order=3, GroupName="Parameters")]
		public int ValueArea
		{ get; set; }
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private MyCustomIndicator[] cacheMyCustomIndicator;
		public MyCustomIndicator MyCustomIndicator(int lookback, int rows, int valueArea)
		{
			return MyCustomIndicator(Input, lookback, rows, valueArea);
		}

		public MyCustomIndicator MyCustomIndicator(ISeries<double> input, int lookback, int rows, int valueArea)
		{
			if (cacheMyCustomIndicator != null)
				for (int idx = 0; idx < cacheMyCustomIndicator.Length; idx++)
					if (cacheMyCustomIndicator[idx] != null && cacheMyCustomIndicator[idx].Lookback == lookback && cacheMyCustomIndicator[idx].Rows == rows && cacheMyCustomIndicator[idx].ValueArea == valueArea && cacheMyCustomIndicator[idx].EqualsInput(input))
						return cacheMyCustomIndicator[idx];
			return CacheIndicator<MyCustomIndicator>(new MyCustomIndicator(){ Lookback = lookback, Rows = rows, ValueArea = valueArea }, input, ref cacheMyCustomIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MyCustomIndicator MyCustomIndicator(int lookback, int rows, int valueArea)
		{
			return indicator.MyCustomIndicator(Input, lookback, rows, valueArea);
		}

		public Indicators.MyCustomIndicator MyCustomIndicator(ISeries<double> input , int lookback, int rows, int valueArea)
		{
			return indicator.MyCustomIndicator(input, lookback, rows, valueArea);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MyCustomIndicator MyCustomIndicator(int lookback, int rows, int valueArea)
		{
			return indicator.MyCustomIndicator(Input, lookback, rows, valueArea);
		}

		public Indicators.MyCustomIndicator MyCustomIndicator(ISeries<double> input , int lookback, int rows, int valueArea)
		{
			return indicator.MyCustomIndicator(input, lookback, rows, valueArea);
		}
	}
}

#endregion
