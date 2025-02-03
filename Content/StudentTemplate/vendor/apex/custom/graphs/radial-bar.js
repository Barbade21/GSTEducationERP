var options = {
	series: [40, 50, 60, 70, 80],
	chart: {
		height: 240,
		type: "radialBar",
	},
	plotOptions: {
		radialBar: {
			dataLabels: {
				name: {
					fontSize: "22px",
				},
				value: {
					fontSize: "16px",
				},
				total: {
					show: true,
					label: "Total",
					formatter: function (w) {
						// By default this function returns the average of all series. The below is just an example to show the use of custom formatter function
						return 249;
					},
				},
			},
		},
	},
	labels: ["Samsung", "Apple", "Nokia", "Motorola", "Huawei"],
	colors: [
		"#2f477a",
		"#35508a",
		"#3b5999",
		"#4f6aa3",
		"#627aad",
		"#768bb8",
		"#899bc2",
		"#9daccc",
	],
};

var chart = new ApexCharts(document.querySelector("#radial"), options);
chart.render();
