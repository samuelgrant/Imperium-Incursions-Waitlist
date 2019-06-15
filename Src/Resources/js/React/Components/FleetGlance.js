import React, { Component } from 'react';

export default class FleetGlance extends Component {
    getPilots() {
        if (this.props.members && this.props.members.pilots)
            return this.props.members.pilots;

        return null;
    }

    getComp(pilots) {
        var dict = new Object();

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


            //>> map object into an array if queue ID matches id in state
        }


        return dict;
    }

    render() {
        if (this.getPilots())
            console.log(this.getComp(this.getPilots()))
        //console.log(this.getComp(this.getPilots()))
        
        return (
            <div className="row">
                <GlanceMenu />

                <GlanceComp />
            </div>
        )
    }
}

export class GlanceComp extends Component {
    render() {
        return (<span>Glance Here</span>);
    }
}

export class GlanceMenu extends Component {
    render() {
        return (
            <div className="col-12">
                <ul class="nav nav-tabs nav-justified">
                    <li className="nav-item">
                        <a role="tab" data-toggle="tab" className="nav-link active" href="#tab-1">Fleet</a>
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
                        <a role="tab" data-toggle="tab" className="nav-link" href="#tab-3">Support</a>
                        <span className="badge badge-dark">42</span>
                    </li>
                </ul>
            </div>
        )
    }
}