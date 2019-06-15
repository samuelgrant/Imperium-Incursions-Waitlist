import React, { Component } from 'react';
import ReactTooltip from 'react-tooltip'

export default class FleetGlance extends Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedQueue: 0
        };
    }

    getPilots() {
        if (this.props.members && this.props.members.pilots)
            return this.props.members.pilots;

        return null;
    }

    getComp(pilots) {
        var dict = new Object();

        if (pilots == null)//>>> Can I look up pilots here instead of having it's own method, and wrap the whole dict logic inside of an if assuming we have pilots?
            return null;

        for (let i = 0; i < pilots.length; i++) {
            let ship = pilots[i].ship;

            if (dict[ship.id]) {
                dict[ship.id].pilots.push(pilots[i].name);
            } else {
                dict[ship.id] = {
                    id: ship.id,
                    name: ship.name,
                    queue: ship.queue,
                    pilots: [pilots[i].name]
                }
            }

            // Alphabetical sort of pilots in each ship
            Object.keys(dict).forEach(function (key) {
                dict[key].pilots.sort();
            });
        }

        return dict;
    }

    getFilteredComp() {
        let filteredComp = [];
        let comp = this.getComp(this.getPilots());

        if (comp == null)
            return null;

        var xxx = Object.keys(comp).map(function (key) {
            return [comp[key]];
        });

        for (let i = 0; i < xxx.length; i++) {
            if (this.state.selectedQueue == 0 || xxx[i][0].queue.id == this.state.selectedQueue) {
                filteredComp.push(xxx[i]);
            }
        }

        return filteredComp;
    }

    setFilter(filterId) {
        this.setState({ selectedQueue: filterId });
    }

    render() {       
        return (
            <div className="row">
                <GlanceMenu updateFilter={this.setFilter.bind(this)}/>

                <GlanceComp comp={this.getFilteredComp()} />
            </div>
        )
    }
}

export class GlanceComp extends Component {
    render() {
        let ships;
        if (this.props.comp) {
            ships = this.props.comp.map((ship) => {
                let pilots = ship[0].pilots.join("<br>");


                return (
                    <div className="col-lg-6 p-3">
                        <img class="rounded pr-3" src={`https://image.eveonline.com/Render/${ship[0].id}_32.png`} alt={ship[0].name} />
                        <p className="d-inline pr-3" data-tip={pilots} data-multiline="true">{ship[0].name}</p>
                        <span className="badge badge-warning">{ship[0].pilots.length}</span>
                        <ReactTooltip />
                    </div>
                )
            })
        }


        return (
            <div className="col-12">
                <div className="row">
                    {ships}
                </div>
            </div>
        );
    }
}

export class GlanceMenu extends Component {
    render() {
        return (
            <div className="col-12">
                <ul class="nav nav-tabs nav-justified">
                    <li className="nav-item">
                        <a role="tab" data-toggle="tab" className="nav-link active" href="#tab-1" onClick={this.props.updateFilter.bind(this, 0)}>Fleet</a>
                        <span className="badge badge-warning">42</span>
                    </li>
                    <li className="nav-item">
                        <a role="tab" data-toggle="tab" className="nav-link" href="#tab-2">DPS</a>
                        <span className="badge badge-dark">42</span>
                    </li>
                    <li className="nav-item">
                        <a role="tab" data-toggle="tab" className="nav-link" href="#tab-3">Logi</a>
                        <span className="badge badge-dark">42</span>
                    </li>
                    <li className="nav-item">
                        <a role="tab" data-toggle="tab" className="nav-link" href="#tab-3">Capital</a>
                        <span className="badge badge-dark">42</span>
                    </li>
                    <li className="nav-item">
                        <a role="tab" data-toggle="tab" className="nav-link" href="#tab-3">Fax</a>
                        <span className="badge badge-dark">42</span>
                    </li>
                    <li class="nav-item">
                        <a role="tab" data-toggle="tab" className="nav-link" href="#tab-3" onClick={this.props.updateFilter.bind(this, 5)}>Support</a>
                        <span className="badge badge-dark">42</span>
                    </li>
                </ul>
            </div>
        )
    }
}