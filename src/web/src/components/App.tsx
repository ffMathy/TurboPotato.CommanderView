import * as React from "react";

export interface AppProps {
}

export interface AppState {
    players: Player[],
    tileMatrix: TileType[][]
}

enum TileType {
    Empty = 0,
    Wall = 1
}

interface Position {
    x: number;
    y: number;

}

interface Player {
    id: string;
    position: Position;
}

interface MapResult {
    players: Player[],
    tileMatrix: TileType[][]
}

const ClassNames = {
    0: "Empty",
    1: "Wall"
}

const apiUrl = "http://10.204.26.143:5000/api/map";

export default class App extends React.Component<AppProps, AppState> {
    constructor(props: AppProps) {
        super(props);
        this.fetchData = this.fetchData.bind(this);
        this.state = { players: [], tileMatrix: [] };
    }
    render() {

        const rows = this.state.tileMatrix;
        const matrix = rows.map(row => {

            const columnTds = row.map(column => {
                const className = ClassNames[column];
                return < td className={`column ${className}`} />;
            })
            return <tr>{columnTds}</tr>;

        })
        return <table>{matrix}</table>;
    }

    componentDidMount() {
        setInterval(this.fetchData, 100);
    }

    fetchData() {
        fetch(apiUrl).then(response => response.json()).then(json => json as MapResult).then(mapResult => {
            this.setState({
                players: mapResult.players,
                tileMatrix: mapResult.tileMatrix
            })
        });
    }
}
