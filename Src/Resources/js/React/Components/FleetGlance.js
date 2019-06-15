import React, { Component } from 'react';
import ReactTooltip from 'react-tooltip'

export default class FleetGlance extends Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedQueue: 0
        };
    }

    getComp() {
        if (!this.props.members) return null;

        let pilots = this.props.members.pilots;
        var dict = new Object();

        // Create a dictionary of ships
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


        ///>> TURN IT INTO AN ARRAY
        return Object.keys(dict).map(function (key) {
            return dict[key];
        });
    }

    getFilteredComp() {
        let filteredComp = [];
        let comp = this.getComp();

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

    getFilters() {
        let fleetComp = this.getComp();
        let queues = []

        if (fleetComp == null) return null;

        for (let i = 0; i < fleetComp.length; i++) {

            if (queues[fleetComp[i].queue.id]) {
                let tmp = queues[fleetComp[i].queue.id];
                tmp.count = tmp.count + fleetComp[i].pilots.length;
                queues[fleetComp[i].queue.id] = tmp;
            } else {
                queues[fleetComp[i].queue.id] = { id: fleetComp[i].queue.id, name: fleetComp[i].queue.name, count: fleetComp[i].pilots.length };
            }
        }

        return queues;
    }

    setFilter(filterId) {
        this.setState({ selectedQueue: filterId });
    }

    render() {       
        return (
            <div className="row">
                <GlanceMenu filters={this.getFilters()} activeFilterId={this.state.selectedQueue} updateFilter={this.setFilter.bind(this)}/>

                <GlanceComp comp={this.getFilteredComp()} />
            </div>
        )
    }
}
//>> TODO: CLean the [0] from the code
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
                        <span className="badge badge-warning float-right">{ship[0].pilots.length}</span>
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
        let tabs;
        if (this.props.filters != null) {
            tabs = this.props.filters.map((filter) => {
                let active = (filter.id == this.props.activeFilterId) ? 'active' : '';

                return (
                    <li className="nav-item">
                        <a role="tab" data-toggle="tab" className={`nav-link ${active}`} href="#tab-1" onClick={this.props.updateFilter.bind(this, filter.id)}>{filter.name}</a>
                        <span className="badge badge-warning">{filter.count}</span>
                    </li>
                )
            });
        }
        return (
            <div className="col-12">
                <ul class="nav nav-tabs nav-justified">
                    {tabs}
                </ul>
            </div>
        )
    }
}