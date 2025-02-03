// Sparkline 1
var options1 = {
	series: [80],
	chart: {
		type: "radialBar",
		width: 70,
		height: 70,
		sparkline: {
			enabled: true,
		},
	},
	dataLabels: {
		enabled: false,
	},
	colors: ["#a7874b"],
	plotOptions: {
		radialBar: {
			hollow: {
				margin: 0,
				size: "50%",
			},
			track: {
				background: "#cdd1dd",
				margin: 0,
			},
			dataLabels: {
				show: false,
			},
		},
	},
};
var chart1 = new ApexCharts(document.querySelector("#coffee"), options1);
chart1.render();

// Sparkline 2
var options2 = {
	series: [70],
	chart: {
		type: "radialBar",
		width: 70,
		height: 70,
		sparkline: {
			enabled: true,
		},
	},
	dataLabels: {
		enabled: false,
	},
	colors: ["#3b5999"],
	plotOptions: {
		radialBar: {
			hollow: {
				margin: 0,
				size: "50%",
			},
			track: {
				background: "#cdd1dd",
				margin: 0,
			},
			dataLabels: {
				show: false,
			},
		},
	},
};
var chart2 = new ApexCharts(document.querySelector("#cakes"), options2);
chart2.render();

// Sparkline 3
var options3 = {
	series: [60],
	chart: {
		type: "radialBar",
		width: 70,
		height: 70,
		sparkline: {
			enabled: true,
		},
	},
	dataLabels: {
		enabled: false,
	},
	colors: ["#3d3f45"],
	plotOptions: {
		radialBar: {
			hollow: {
				margin: 0,
				size: "50%",
			},
			track: {
				background: "#cdd1dd",
				margin: 0,
			},
			dataLabels: {
				show: false,
			},
		},
	},
};
var chart3 = new ApexCharts(document.querySelector("#burgers"), options3);
chart3.render();
