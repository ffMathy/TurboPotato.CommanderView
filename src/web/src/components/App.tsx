import * as React from "react";
import { getColor } from "../color-helper";

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
    cellPosition: Position;
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



const Player = (props: { id?: string, position: Position }) => {
    const { position } = props;
    const left = (position.x - Math.floor(position.x)) * 10;
    const top = (position.y - Math.floor(position.y)) * 10;
    const color = getColor(props.id);
    console.log(color);
    return <div className="player" style={{ backgroundColor: color, position: "absolute", left: left + "%", top: top + "%" }}></div>
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props: AppProps) {
        super(props);
        this.fetchData = this.fetchData.bind(this);
        this.state = { players: [], tileMatrix: [] };
    }
    render() {

        const rows = this.state.tileMatrix;
        const players = this.state.players;
        const matrix = rows.map((row, rowIndex) => {
            const columnTds = row.map((column, columnIndex) => {
                const className = ClassNames[column];
                const ourPlayers = players.filter(player => player.cellPosition.x === rowIndex && player.cellPosition.y === columnIndex);
                return < td className={`column ${className}`} >
                    {ourPlayers.map(player => <Player id={player.id} position={player.position} />)}
                </ td>;
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
                players: mapResult.players.map((player) => {
                    return { ...player, cellPosition: { x: Math.floor(player.position.x), y: Math.floor(player.position.y) } }
                }),
                tileMatrix: mapResult.tileMatrix
            })
        });
    }
}
