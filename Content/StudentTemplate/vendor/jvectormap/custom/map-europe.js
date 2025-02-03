// Europe
$(function () {
	$("#mapEurope").vectorMap({
		map: "europe_mill",
		zoomOnScroll: false,
		series: {
			regions: [
				{
					values: gdpData,
					scale: ["#bf7a6a"],
					normalizeFunction: "polynomial",
				},
			],
		},
		backgroundColor: "transparent",
	});
});
