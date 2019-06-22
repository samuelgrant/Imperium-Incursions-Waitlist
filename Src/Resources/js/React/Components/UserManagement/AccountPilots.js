import React, { Component } from 'react';
import { render } from 'react-dom';
import { Pilot } from '../EsiUi';

export default class AccountPilots extends Component {

    render() {
        let pilots;
        if (this.props.pilots) {
            pilots = this.props.pilots.map((pilot) => {
                return (
                    <div className="col-lg-6 col-sm-12 pb-3">
                        <img className="float-left pr-3" src={`https://imageserver.eveonline.com/Character/${pilot.id}_32.jpg`} alt="Pilot Avatar" />
                        <Pilot pilot={pilot} />
                    </div>
                )
            });
        }

        return (
            <div>
                <h4>Pilots</h4>
                <div className="row">
                    {pilots}
                </div>
            </div>
        )
    }
}