// Sparkline 1
var options1 = {
	series: [
		{
			name: "Coffee",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	chart: {
		type: "line",
		width: 70,
		height: 50,
		sparkline: {
			enabled: true,
		},
	},
	stroke: {
		curve: "smooth",
		width: 5,
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	tooltip: {
		fixed: {
			enabled: false,
		},
	},
	colors: ["#a7874b"],
	xaxis: {
		categories: [
			"Sunday",
			"Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday",
		],
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};
var chart1 = new ApexCharts(document.querySelector("#coffee"), options1);
chart1.render();

// Sparkline 2
var options2 = {
	series: [
		{
			name: "Cakes",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	chart: {
		type: "line",
		width: 70,
		height: 50,
		sparkline: {
			enabled: true,
		},
	},
	stroke: {
		curve: "smooth",
		width: 5,
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	tooltip: {
		fixed: {
			enabled: false,
		},
	},
	colors: ["#3b5999"],
	xaxis: {
		categories: [
			"Sunday",
			"Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday",
		],
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};
var chart2 = new ApexCharts(document.querySelector("#cakes"), options2);
chart2.render();

// Sparkline 3
var options3 = {
	series: [
		{
			name: "Burgers",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	chart: {
		type: "line",
		width: 70,
		height: 50,
		sparkline: {
			enabled: true,
		},
	},
	stroke: {
		curve: "smooth",
		width: 5,
	},
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	tooltip: {
		fixed: {
			enabled: false,
		},
	},
	colors: ["#1fa278"],
	xaxis: {
		categories: [
			"Sunday",
			"Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday",
		],
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};
var chart3 = new ApexCharts(document.querySelector("#burgers"), options3);
chart3.render();
