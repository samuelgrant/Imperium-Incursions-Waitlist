import React, { Component } from 'react';
import { Input } from './FormControls'



export class Account extends Component {
    render() {
        return (
            <div className="form-group">
                <label htmlFor="#lookup_account">GSF Auth Name:</label>
                <Input id="lookup_account" type="text" classOverride="form-control account-lookup" value={this.props.value} name="name" required="true"/>
            </div>
        )
    }
}

export class Pilot extends Component {
    render() {
        return (
            <div className="form-group">
                <label htmlFor="#lookup_pilot">Pilot Name:</label>
                <input id="lookup_pilot" className="form-control pilot-lookup" type="text" name="name" required/>
            </div>
        )
    }
}

export class AccountPilot extends Component {
    render() {
        return (
            <div className="form-group">
                <label htmlFor="#lookup_pilotOrAccount">GSF Auth or Pilot Name:</label>
                <input id="lookup_pilotOrAccount" className="form-control lookup" type="text" name="name" required/>
            </div>
        )
    }
}