declare module cfx {
    class Chart {
        constructor();

        setGallery(galeryType: Gallery): void;
        getAllSeries();
        getLegendBox();
        setDataSource(data: any[]);
        getPoints();
        getDataGrid();
        create(element: any);
        getPanes();
        getAnimations();
        getSeries();
        getTitles();
        getGalleryAttributes();
        getAxisY();
        getAxisX();
    }
    class Pane {}
    class TitleDockable {}

    enum Gallery {
        Area,
        Bar,
        Bubble,
        Candlestick,
        Contour,
        Cube,
        Curve,
        CurveArea,
        Doughnut,
        Gantt,
        HighLowClose,
        Lines,
        Pareto,
        Pie,
        Pyramid,
        Radar,
        Scatter,
        Step,
        Surface
    }

    enum Stacked {
        Stacked100,
        Normal
    }

    enum DockBorder {
        External,
        Internal,
        None
    }

    enum DockArea {
        Bottom,
        Left,
        Right,
        Top
    }

    enum FillMode {
        Gradient,
        Monochrome,
        Pattern,
        Solid
    }

    enum MarkerShape {
        Rect
    }
    enum Interlaced {
        None,
        Horizontal,
        Vertical
    }
    enum ContentLayout {
        Center,
        Far,
        Near,
        Spread
    }
    enum AxisFormat {
        Currency,
        Date,
        Date_Time,
        Long_Date,
        None,
        Number,
        Percentage,
        Scientific,
        Time
    }
}