var options = {
	chart: {
		height: 360,
		type: "bar",
		toolbar: {
			show: false,
		},
	},
	plotOptions: {
		bar: {
			columnWidth: "20%",
			borderRadius: 4,
			distributed: true,
			dataLabels: {
				position: "center",
			},
		},
	},
	series: [
		{
			name: "Balance",
			data: [52, 73, 34, 66, 82, 49, 38, 59, 44, 86, 30, 60],
		},
	],
	legend: {
		show: false,
	},
	xaxis: {
		categories: [
			"Jan",
			"Feb",
			"Mar",
			"Apr",
			"May",
			"Jun",
			"Jul",
			"Aug",
			"Sep",
			"Oct",
			"Nov",
			"Dec",
		],
		axisBorder: {
			show: false,
		},
		yaxis: {
			show: false,
		},

		tooltip: {
			enabled: true,
		},
		labels: {
			show: true,
			rotate: -45,
			rotateAlways: true,
		},
	},
	grid: {
		borderColor: "#dfd6ff",
		strokeDashArray: 5,
		xaxis: {
			lines: {
				show: true,
			},
		},
		yaxis: {
			lines: {
				show: false,
			},
		},
		padding: {
			top: 0,
			right: 0,
			bottom: 0,
		},
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + " million";
			},
		},
	},
	colors: [
		"#2f477a",
		"#768bb8",
		"#a7874b",
		"#3b5999",
		"#627aad",
		"#4f6aa3",
		"#899bc2",
		"#9daccc",
	],
};
var chart = new ApexCharts(document.querySelector("#yearlySales"), options);
chart.render();
